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
            if($line.StartsWith('From')){
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
            }
            
            $msg.Content += $line
        }
        $r.Dispose()
    }
    End {
        
    }
}
Function Convert-MboxToTextFiles {
    <#
    .SYNOPSIS
    Splits up a .mbox file to .txt files for each message
    
    .DESCRIPTION
    Splits up a .mbox file to .txt files for each message
    
    .PARAMETER MboxPath
    Path to .mbox file
    
    .PARAMETER OutputPath
    Directory to place message .txt files
    
    .EXAMPLE
     Convert-MboxToTextFiles -MboxPath $path -OutputPath $out
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true)]
        [ValidateScript( {Test-Path $_ -PathType Leaf} )]
        [string]$MboxPath,
        [Parameter(Mandatory=$true)]
        [ValidateScript( {Test-Path $_ -PathType Container} )]
        [string]$OutputPath
    )
    $id = 0
    Get-MboxMessage $MboxPath | ForEach-Object {
        $file = ($OutputPath+"\message_$id.txt")
        $_.Content | Out-File $file
        $id++
    }
}

Export-ModuleMember -Function *-*
