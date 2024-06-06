using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Resumer.models;

public enum CommandDisplay
{
    Hidden,
    Attached,
    Detached,
    Verbose,
}

/// <summary>
/// Models the Typst CLI command
/// </summary>
internal partial class Command //: IDisposable, IAsyncDisposable
{
    private CommandDisplay _commandDisplay = CommandDisplay.Attached;

    private uint _onStandardErrorListeners = 0;

    private uint _onStandardOutputListeners = 0;

    public Command(string command)
    {
        FileName = command;
        Process = new CommandProcess(StartInfo);
    }

    private CommandProcess Process { get; set; }
    private ProcessStartInfo StartInfo { get; set; } = new();
    public ProcessState ProcessState => Process.State;

    /// <summary>
    /// Allow for programmatic input: the use of the <see cref="Input"/> method
    /// </summary>
    public bool RedirectStandardInput
    {
        get => StartInfo.RedirectStandardInput;
        set => StartInfo.RedirectStandardInput = value;
    }


    protected string FileName
    {
        get => StartInfo.FileName;
        set
        {
            value = value.Trim();
            if(OperatingSystem.IsWindows() && !EndsWithExeRegex().Match(value).Success)
                value += ".exe";
            StartInfo.FileName = value;
        }
    }

    public string WorkingDirectory
    {
        get => StartInfo.WorkingDirectory;
        set => StartInfo.WorkingDirectory = value;
    }

    /// <summary>
    /// Suppresses command output
    /// If false output is shown
    /// </summary>
    public CommandDisplay CommandDisplay
    {
        get => _commandDisplay;
        set
        {
            _commandDisplay = value;
            switch(CommandDisplay)
            {
                case CommandDisplay.Verbose:
                    StartInfo.ErrorDialog = true;
                    StartInfo.CreateNoWindow = true;
                    StartInfo.UseShellExecute = true;
                    break;
                case CommandDisplay.Hidden:
                    StartInfo.RedirectStandardOutput = false;
                    StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    StartInfo.CreateNoWindow = true;
                    StartInfo.ErrorDialog = false;
                    StartInfo.UseShellExecute = false;
                    break;
                case CommandDisplay.Attached:
                    StartInfo.CreateNoWindow = false;
                    StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    StartInfo.UseShellExecute = false;
                    break;
                case CommandDisplay.Detached:
                    StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    StartInfo.CreateNoWindow = true;
                    StartInfo.UseShellExecute = true;
                    StartInfo.ErrorDialog = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CommandDisplay), CommandDisplay, null);
            }
        }
    }

    /// <summary>
    /// The current state of the command's process
    /// </summary>
    public ProcessState State { get; private set; } = ProcessState.NotStarted;

    public event DataReceivedEventHandler? OnStandardOutput
    {
        add
        {
            if(_onStandardOutputListeners == 0)
                StartInfo.RedirectStandardOutput = true;
            Process.OnStandardOutput += value;
            _onStandardOutputListeners++;
        }
        remove
        {
            _onStandardOutputListeners--;
            if(_onStandardOutputListeners == 0)
                StartInfo.RedirectStandardOutput = false;
            Process.OnStandardOutput -= value;
        }
    }

    public event EventHandler? OnProcessExited
    {
        add => Process.OnProcessExited += value;
        remove => Process.OnProcessExited -= value;
    }

    public event DataReceivedEventHandler? OnStandardError
    {
        add
        {
            if(_onStandardErrorListeners == 0)
                StartInfo.RedirectStandardError = true;
            Process.OnStandardError += value;
            _onStandardErrorListeners++;
        }
        remove
        {
            _onStandardErrorListeners--;
            if(_onStandardErrorListeners == 0)
                StartInfo.RedirectStandardError = false;
            Process.OnStandardError -= value;
        }
    }

    public bool ExistsOnPath() => GetFullPath() != null;

    public string? GetFullPath()
    {
        if(File.Exists(FileName))
            return Path.GetFullPath(FileName);

        var envVarPath = OperatingSystem.IsWindows()
            ? Environment.GetEnvironmentVariable("Path")
            : Environment.GetEnvironmentVariable("PATH");


        return envVarPath?.Split(Path.PathSeparator)
            .Select(path => Path.Combine(path, FileName))
            .FirstOrDefault(fullPath => File.Exists(fullPath));
    }

    /// <summary>
    /// Starts the process
    /// </summary>
    /// <param name="args">additional arguments to pass to the process</param>
    /// <returns><see cref="CommandProcess"/> representing the (running) process</returns>
    public ICommandProcess Start(params string[] args) => RunProcess(null, false, args);

    /// <summary>Starts the process with a timeout</summary>
    /// <inheritdoc cref="Start(string[])"/>
    /// <param name="timeout">The maximum amount of time to let the process execute before it is stopped</param>
    public ICommandProcess Start(TimeSpan timeout, params string[] args) => RunProcess(timeout, false, args);

    /// <summary>
    /// Run the process until it exits
    /// </summary>
    /// <inheritdoc cref="Start(string[])"/>
    public void Run(params string[] args) => RunProcess(null, true, args);

    /// <summary>
    /// Run the process until it exits or the timeout is reached
    /// </summary>
    /// <inheritdoc cref="Run(string[])"/>
    /// <param name="timeout">the maximum amount of time to let the process run until it is stopped</param>
    public void Run(TimeSpan timeout, params string[] args) => RunProcess(timeout, true, args);

    private ICommandProcess RunProcess(TimeSpan? timeout, bool waitForExit, params string[] args)
    {
        if(waitForExit)
            return timeout is null ? Process.Start(args).Complete() : Process.Start(args).Complete(timeout.Value);

        return timeout is null ? Process.Start(args) : Process.Run(timeout.Value, args);
    }

    [GeneratedRegex(".*\\.exe", RegexOptions.IgnoreCase, "en-CA")]
    private static partial Regex EndsWithExeRegex();


    private sealed class CommandProcess: ICommandProcess //: IDisposable, IAsyncDisposable
    {
        private uint _onStandardErrorHandlerCount = 0;
        private uint _onStandardOutputHandlerCount = 0;

        public CommandProcess(ProcessStartInfo startInfo)
        {
            Process.StartInfo = startInfo;
            Process.EnableRaisingEvents = true;
            Process.Exited += (sender, args) =>
            {
                State = ProcessState.Exited;
                Process.StartInfo.Arguments = string.Empty;
                if(_onStandardOutputHandlerCount > 0)
                    Process.CancelOutputRead();
                if(_onStandardErrorHandlerCount > 0)
                    Process.CancelErrorRead();
            };
        }

        private System.Diagnostics.Process Process { get; set; } = new();

        public ProcessState State { get; private set; } = ProcessState.NotStarted;

        public ICommandProcess Run(TimeSpan timeout, params string[] args)
        {
            Start(args);
            return Complete(timeout);
        }

        public ICommandProcess Start(params string[] args)
        {
            Process.StartInfo.Arguments = string.Join(' ', args);
            Process.Start();
            State = ProcessState.Running;

            if(Process.StartInfo.RedirectStandardInput && _onStandardOutputHandlerCount > 0)
                Process.BeginOutputReadLine();
            if(Process.StartInfo.RedirectStandardError && _onStandardErrorHandlerCount > 0)
                Process.BeginErrorReadLine();

            if(Process.StartInfo.RedirectStandardInput)
                Process.StandardInput.AutoFlush = true;
            return this;
        }

        public ICommandProcess Input(string data)
        {
            try
            {
                Process.StandardInput.WriteLine(data);
            }
            catch(InvalidOperationException exception)
            {
                string msg;
                if(State != ProcessState.Running)
                {
                    msg = "Process is not running: ";
                    if(State == ProcessState.NotStarted)
                        msg += "the process has not yet been started";
                    else
                        msg += "the process has already exited";
                }
                else if(!Process.StartInfo.RedirectStandardInput)
                    msg = "The process is not configured to redirect standard input";
                else
                    msg = "standard input must be redirected before the process has started";

                throw new InvalidOperationException(msg, exception);
            }

            return this;
        }

        public ICommandProcess Complete()
        {
            if(Process.StartInfo.RedirectStandardInput)
                Process.StandardInput.Close();
            Process.WaitForExit();
            State = ProcessState.Exited;
            return this;
        }

        public ICommandProcess Complete(TimeSpan timeout)
        {
            if(Process.StartInfo.RedirectStandardInput)
                Process.StandardInput.Close();
            if(!Process.WaitForExit(timeout))
            {
                Process.Kill();
                State = ProcessState.Killed;
            }
            else
                State = ProcessState.Exited;

            return this;
        }

        protected internal event DataReceivedEventHandler OnStandardOutput
        {
            add
            {
                Process.OutputDataReceived += value;
                _onStandardOutputHandlerCount++;
            }
            remove
            {
                Process.OutputDataReceived -= value;
                _onStandardOutputHandlerCount--;
            }
        }

        protected internal event EventHandler? OnProcessExited
        {
            add => Process.Exited += value;
            remove => Process.Exited -= value;
        }

        protected internal event DataReceivedEventHandler? OnStandardError
        {
            add
            {
                Process.ErrorDataReceived += value;
                _onStandardErrorHandlerCount++;
            }
            remove
            {
                Process.ErrorDataReceived -= value;
                _onStandardErrorHandlerCount--;
            }
        }
    }
}

public interface ICommandProcess
{
    public ProcessState State { get; }
    public ICommandProcess Run(TimeSpan timeout, params string[] args);
    public ICommandProcess Start(params string[] args);

    /// <summary>
    /// send data to the process stdin
    /// </summary>
    /// <remarks>Requires parent <see cref="Command"/>.<see cref="RedirectStandardInput"/> to be true before calling and <see cref="Start"/> to be called beforehand</remarks>
    /// <param name="data">input data to pass to the process stdin</param>
    public ICommandProcess Input(string data);

    /// <summary>
    /// Waits for the process to exit
    /// </summary>
    /// <remarks>closes the standard input stream if it has been redirected</remarks>
    /// <returns>the exited process</returns>
    public ICommandProcess Complete();

    /// <summary>
    /// waits for the process to exit or foricbly closes it after a timeout
    /// </summary>
    /// <param name="timeout">the maximum time to wait for the process to exit</param>
    /// <inheritdoc cref="Complete()"/>
    public ICommandProcess Complete(TimeSpan timeout);
}

public enum ProcessState
{
    NotStarted,
    Exited,
    Running,
    Killed
}