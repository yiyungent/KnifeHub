

$current_dir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
$log_file = "${current_dir}\monitor.log"
# 脚本日志最大为10M
$log_max_size = 10*1024*1024

# 需要检测的进程名和启动文件路径
$process_name = "KnifeHub.Web"
$start_up_file = "KnifeHub.Web.exe"



#此例子为每10秒一次的定时任务，通过设置$step和$add可以实现延时执行任务。 
function waitsec{
	$step=10 #设置间隔
	$add=0 #设置延时
	$t=(get-date)
	$step-(($t.Hour*3600+$t.Minute*60+$t.Second)%$step)+$add
}
 
write-host "running...... please wait" (waitsec)"S" 
Start-Sleep -s (waitsec)
while(1){
	start
	
	Start-Sleep -s (waitsec)
}



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


function start()
{
    $process_name = "KnifeHub.Web"
	$start_up_file = "KnifeHub.Web.exe"
	Get-Process | findstr "KnifeHub.Web" > $null
	if ( $? -eq "True" )
	{
		#log "process ${process_name} is running!"
	}
	else
	{
		#log "process ${process_name} is not exist, now to start it."
		Start-Process -FilePath "KnifeHub.Web.exe"
		if ( $? -eq "True" )
		{
			#log "start ${process_name} succefully!"
		}
		else
		{
			#log "start ${process_name} failed!"
		}
	}
}












