using System;

namespace MediaInfoKeeper.Services {
    /// <summary>
    ///     插件自动更新所使用的 GitHub 仓库。
    ///     默认指向本插件的发行仓库，可通过“更新源仓库”选项改成自己的 fork，
    ///     避免自动更新把本地改动覆盖回上游版本。
    /// </summary>
    internal static class PluginUpdateSource {
        public const string DefaultRepository = "qydxd1/MediaInfoKeeper";

        private const string VersionManifestBranch = "master";

        /// <summary>
        ///     把用户填写的仓库值规范化为 owner/repo。
        ///     支持完整链接（https://github.com/owner/repo、结尾 .git、多余路径段），
        ///     无法识别时回退到 <see cref="DefaultRepository" />。
        /// </summary>
        public static string NormalizeRepository(string repository) {
            var value = (repository ?? string.Empty).Trim();
            if (value.Length == 0) return DefaultRepository;

            const string httpsPrefix = "https://github.com/";
            const string httpPrefix = "http://github.com/";
            if (value.StartsWith(httpsPrefix, StringComparison.OrdinalIgnoreCase))
                value = value.Substring(httpsPrefix.Length);
            else if (value.StartsWith(httpPrefix, StringComparison.OrdinalIgnoreCase))
                value = value.Substring(httpPrefix.Length);

            value = value.Trim().Trim('/');
            if (value.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                value = value.Substring(0, value.Length - 4);

            var segments = value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2) return DefaultRepository;

            var owner = segments[0].Trim();
            var repositoryName = segments[1].Trim();
            if (owner.Length == 0 || repositoryName.Length == 0) return DefaultRepository;

            foreach (var character in owner + repositoryName)
                if (char.IsWhiteSpace(character))
                    return DefaultRepository;

            return owner + "/" + repositoryName;
        }

        public static string BuildReleasesUrlTemplate(string repository, int pageSize) {
            return $"https://api.github.com/repos/{NormalizeRepository(repository)}/releases?per_page={pageSize}&page={{0}}";
        }

        public static string BuildVersionManifestUrl(string repository) {
            return $"https://raw.githubusercontent.com/{NormalizeRepository(repository)}/{VersionManifestBranch}/Version.json";
        }
    }
}
