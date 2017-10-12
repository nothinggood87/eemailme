# Various PowerShell functions for working with mail.
Class MboxMessage {
    [string[]]$Content
}
Function Get-MboxMessage {
    <#
    .SYNOPSIS
    Gets messages out of a .mbox file
    
    .DESCRIPTION
    Gets messages out of a .mbox file
    
    .PARAMETER Path
    Path to .mbox file
    
    .PARAMETER First
    Limit to first count of messages
    
    .EXAMPLE
    An example
    
    .NOTES
    General notes
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript( {Test-Path $_} )]
        [string]$Path
    )
    Begin {
        $msg = [MboxMessage]::new()
        $first = $true
        $count = 0
    }
    Process {
        $r = [IO.File]::OpenText($Path)
        while ($r.Peek() -ge 0) {
            $line = $r.ReadLine()
            
            # if ($count -gt 557) {
            #     Write-Verbose $line
            # }
            if ($line -match 'From [0-9]+@xxx [a-zA-Z]{3} [a-zA-Z]{3} [0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2} \+0000 [0-9]{4}') {
                $count++
                $pipe = $msg
                $msg = [MboxMessage]::new()
                if ($first) {
                    $first = $false
                } else {
                    $pipe
                }
            }
            $msg.Content += $line
        }
        $r.Dispose()
    }
    End {
        
    }
}

Export-ModuleMember -Function *-*
