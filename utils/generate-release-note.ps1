
# 从环境变量中获取
# $projectTagName = "QQChannelPlugin"
$projectTagName = $env:GitProjectTagName
# $projectDir = "plugins/QQChannelPlugin/*"
$projectPath = $env:GitProjectPath

# 一定要确保在仓库根目录执行

$targetTags = git tag | where { $_ -like "${projectTagName}-v*" }

$projectTagScope = $targetTags[$targetTags.Count - 2] + ".." + $targetTags[$targetTags.Count - 1]


$projectLogs = git log $projectTagScope --format=%H----DELIMITER----%s $projectPath

# TODO: 顺序可能不一致, 导致无法一一对应
# $projectCommitIds = git log $projectTagScope --format=%H $projectPath
# $projectMessages = git log $projectTagScope --format=%s $projectPath
# $projectCommitIds
# $projectMessages

$projectCommits = @()

for ($i = 0; $i -lt $projectLogs.Count; $i++) {

    $itemTemp = $projectLogs[$i] -split "----DELIMITER----"
    $commitId = $itemTemp[0]
    $message = $itemTemp[1]

    # $itemObject = New-Object PSObject –Property @{CommitId = $projectCommitIds[$i]; Message = $projectMessages[$i - 1] }
    $itemObject = New-Object -TypeName PSObject
    $itemObject | Add-Member -Name 'CommitId' -MemberType Noteproperty -Value $commitId
    $itemObject | Add-Member -Name 'Message' -MemberType Noteproperty -Value $message
    $projectCommits += $itemObject
}

$projectCommits | Format-Table -Auto

# TODO: 若需要用到 projectLatestTag
$projectLatestTag = $targetTags[$targetTags.Count - 1]

$tempReleaseNoteFilePath = "temp-release-note.md"
"" > $tempReleaseNoteFilePath

foreach ($currentItem in $projectCommits) {
    $iteCommitId = $currentItem.CommitId
    $itemMessage = $currentItem.Message
    "- ${itemMessage} (${iteCommitId})  " >> $tempReleaseNoteFilePath
}