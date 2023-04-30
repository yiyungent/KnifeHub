class GitHubReposViewModel {
    owner: string = "";
    repository: string = "";
    releases: GitHubReleaseItemModel[] = [];
}

class GitHubReleaseItemModel {
    tagName: string = "";
    releaseName: string = "";
    note: string = "";
    createTime: Date = new Date();
    assets: GitHubAssetItemModel[] = [];
}

class GitHubAssetItemModel {
    fileName: string = "";
    fileDownloadUrl: string = "";
    fileSize: string = "";
    createTime: Date = new Date();
}

export default GitHubReposViewModel;