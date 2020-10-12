<#
.SYNOPSIS
    Gets file encoding.
.DESCRIPTION
    The Get-FileEncoding function determines encoding by looking at Byte Order Mark (BOM).
    Based on port of C# code from http://www.west-wind.com/Weblog/posts/197245.aspx
.OUTPUTS
    System.Text.Encoding
.PARAMETER Path
    The Path of the file that we want to check.
.PARAMETER DefaultEncoding
    The Encoding to return if one cannot be inferred.
    You may prefer to use the System's default encoding:  [System.Text.Encoding]::Default
    List of available Encodings is available here: http://goo.gl/GDtzj7
.EXAMPLE
    # This command gets ps1 files in current directory where encoding is not ASCII
    Get-ChildItem  *.ps1 | select FullName, @{n='Encoding';e={Get-FileEncoding $_.FullName}} | where {[string]$_.Encoding -ne 'System.Text.ASCIIEncoding'}
.EXAMPLE
    # Same as previous example but fixes encoding using set-content
    Get-ChildItem  *.ps1 | select FullName, @{n='Encoding';e={Get-FileEncoding $_.FullName}} | where {[string]$_.Encoding -ne 'System.Text.ASCIIEncoding'} | foreach {(get-content $_.FullName) | set-content $_.FullName -Encoding ASCII}
.NOTES
    Version History
    v1.0   - 2010/08/10, Chad Miller - Initial release
    v1.1   - 2010/08/16, Jason Archer - Improved pipeline support and added detection of little endian BOMs. (http://poshcode.org/2075)
    v1.2   - 2015/02/03, VertigoRay - Adjusted to use .NET's [System.Text.Encoding Class](http://goo.gl/XQNeuc). (http://poshcode.org/5724)
    v1.2   - 2020/04/03, David Watson - fixed get-Content encoding error to use -AsByteStream + made the comparison actually work.
.LINK
    http://goo.gl/bL12YV
#>
function Get-FileEncoding {
    [CmdletBinding()]
    param (
        [Alias("PSPath")]
        [Parameter(Mandatory = $True, ValueFromPipelineByPropertyName = $True)]
        [String]$Path 
        ,
        [Parameter(Mandatory = $False)]
        [System.Text.Encoding]$DefaultEncoding = [System.Text.Encoding]::ASCII
    )
    
    process {
        [Byte[]]$bom = Get-Content -AsByteStream -ReadCount 4 -TotalCount 4 -Path $Path
        
        $encoding_found = $false
        
        foreach ($encoding in [System.Text.Encoding]::GetEncodings().GetEncoding()) {
            $preamble = $encoding.GetPreamble()
            if ($preamble)
             {
                $maxIndex = $preamble.Length -1
                for($i =0; $i -lt $preamble.Length; $i++)
                {
                    if ($preamble[$i] -ne $bom[$i])
                    {
                        break
                    }
                    elseif ($i -eq $maxIndex)
                    {
                        $encoding_found = $encoding
                        Write-Debug "Found encoding Encoding='$($encoding.BodyName)'"
                    }
                }
            }
        }
        
        if (!$encoding_found)
        {
            Write-Debug "Encoding Not Found Setting default to Encoding='$($DefaultEncoding.BodyName)'"
            $encoding_found = $DefaultEncoding
        }
    
        $encoding_found
    }
}
