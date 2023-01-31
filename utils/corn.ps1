$current_dir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
$log_file = "${current_dir}\monitor.log"
# 脚本日志最大为10M
$log_max_size = 10*1024*1024

# 需要检测的进程名和启动文件路径
$process_name = "KnifeHub.Web"
$start_up_file = "KnifeHub.Web.exe"

function log($content)
{
    $date = Get-Date -UFormat "%Y-%m-%d %H:%M:%S"
    Add-Content -Path $log_file -Value "$date : $content"

    $log_file_size = (Get-ChildItem $current_dir\monitor.log).Length
    if ( $log_file_size -gt $log_max_size)
    {
        if ( Test-Path $current_dir\monitor.log.bak )
        {
            Remove-Item $current_dir\monitor.log.bak
        }
        Copy-Item $log_file $current_dir\monitor.log.bak
        Clear-Content $log_file
    }
}

Get-Process | findstr $process_name > $null
if ( $? -eq "True" )
{
    log "process ${process_name} is running!"
}
else
{
    log "process ${process_name} is not exist, now to start it."
    Start-Process -FilePath $start_up_file
    if ( $? -eq "True" )
    {
        log "start ${process_name} succefully!"
    }
    else
    {
        log "start ${process_name} failed!"
    }
}



