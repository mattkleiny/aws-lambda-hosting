using System.Management.Automation;

namespace Amazon.Lambda.PowerShell
{
  /// <summary>A <see cref="PSCmdlet"/> for invoking a lambda function from PowerShell.</summary>
  [Cmdlet(VerbsLifecycle.Invoke, "Function")]
  public sealed class InvokeFunctionCmdlet : PSCmdlet
  {
    [Parameter(HelpMessage = "The name of hte function to execute", Mandatory = true)]
    public string FunctionName { get; set; }

    protected override void EndProcessing()
    {
      // TODO: think about bootstrapping here
      
      base.EndProcessing();
    }
  }
}