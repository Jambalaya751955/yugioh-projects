using System;
using System.IO;

namespace Snrk.Github
{
    public class GithubRepositoryInfo
    {
        /// <summary>
        /// Username of the repository owner.
        /// </summary>
        public string User
        {
            get { return m_User; }
            set
            {
                if (!IsUsername(value))
                    throw new ArgumentException("Value is not a valid username.");
                m_User = value;
            }
        }
        /// <summary>
        /// Name of the repository.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Repository name cannot be empty.");
                m_Name = value;
            }
        }
        /// <summary>
        /// Name of the branch.
        /// </summary>
        public string Branch
        {
            get { return m_Branch; }
            set { m_Branch = string.IsNullOrEmpty(value) ? "master" : value; }
        }
        /// <summary>
        /// Directory of the Repository
        /// </summary>
        public string Directory
        {
            get { return Path.Combine(m_BaseDirectory, m_Name); }
        }
        /// <summary>
        /// Directory of the repository's folder.
        /// </summary>
        public string BaseDirectory
        {
            get { return m_BaseDirectory; }
            set
            {
                if (string.IsNullOrEmpty(value) || !System.IO.Directory.Exists(value))
                {
                    throw new Exception("Directory does not exist.");
                }
                m_BaseDirectory = value;
            }
        }

        private string m_User;
        private string m_Name;
        private string m_Branch;
        private string m_BaseDirectory;

        /// <summary>
        /// Information for a Github repository.
        /// </summary>
        /// <param name="user">Username of the repository owner.</param>
        /// <param name="name">Name of the repository.</param>
        /// <param name="branch">Name of the branch. "master" if empty.</param>
        /// <param name="directory">Directory of the repository's folder.</param>
        public GithubRepositoryInfo(string user, string name, string branch, string directory)
        {
            this.User = user;
            this.Name = name;
            this.Branch = branch;
            this.BaseDirectory = directory;
        }

        private bool IsUsername(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            bool wasHyphen = false;
            for (int i = 0; i < name.Length; ++i)
            {
                bool isHyphen = name[i] == '-';
                if (!isHyphen && !Char.IsLetterOrDigit(name[i]))
                    return false;
                if ((i == 0 || i == name.Length - 1) && isHyphen)
                    return false;
                if (wasHyphen && isHyphen)
                    return false;
                wasHyphen = isHyphen;
            }

            return true;
        }

        /*private bool IsPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 7)
            {
                return false;
            }

            bool hasLowercaseLetter = false;
            bool hasNumber = false;
            foreach (char c in password)
            {
                if (Char.IsNumber(c))
                    hasNumber = true;
                if (Char.IsLower(c))
                    hasLowercaseLetter = true;
            }

            return hasLowercaseLetter && hasNumber;
        }*/
    }

    public class GithubRepository
    {
        /// <summary>
        /// Information for the repository.
        /// </summary>
        public GithubRepositoryInfo Info;

        /// <summary>
        /// Manager for a Github repository.
        /// </summary>
        public GithubRepository(GithubRepositoryInfo info)
        {
            this.Info = info;
        }

        ~GithubRepository()
        {
            Info = null;
        }
        
        /// <summary>
        /// Checks if the repository has already been cloned.
        /// </summary>
        /// <returns></returns>
        public bool IsCloned()
        {
            string configFilePath = Path.Combine(this.Info.Directory, ".git", "config");
            if (File.Exists(configFilePath))
            {
                string configRepoUrl = null;
                string configFileContent = File.ReadAllText(configFilePath);
                using (StringReader str = new StringReader(configFileContent))
                    for (string line = str.ReadLine(); line != null; line = str.ReadLine())
                        if (line.TrimStart().StartsWith("url"))
                            configRepoUrl = line.RemoveWhitespace().Replace("url=", string.Empty);
                string repoUrl = string.Format("https://github.com/{0}/{1}", this.Info.User, this.Info.Name);
                return configRepoUrl.Contains(repoUrl);
            }
            return false;
        }

        /// <summary>
        /// Clones the repository into the given directory.
        /// </summary>
        /// <param name="override">Override the repository folder if it already exists.</param>
        /// <param name="reset">Reset the repository if it is already cloned.</param>
        /// <returns>
        /// 0: Repository was cloned successfully.
        /// 1: The repository folder already existed and was overriden.
        /// 2: The repository was already cloned and was reseted.
        /// </returns>
        public int Clone(bool @override = false, bool reset = false)
        {
            if (reset && IsCloned())
            {
                _Reset();
                return 2;
            }
            if (@override && System.IO.Directory.Exists(this.Info.Directory))
            {
                DirectoryExt.ForceDelete(this.Info.Directory, true);
                return 1;
            }

            using (var cmd = new CommandPrompt(true))
            {
                cmd.WriteLine("cd {0}", this.Info.BaseDirectory);
                cmd.WriteLine("git clone https://github.com/{0}/{1} --branch {2}", this.Info.User, this.Info.Name, this.Info.Branch);
                cmd.WriteLine("exit");
                
                cmd.RethrowError();
                cmd.WaitForExit();
            }
            return 0;
        }
        
        /// <summary>
        /// Commits one or more files directly to the repository. Installed SSH key is required.
        /// </summary>
        /// <param name="password">Password for the user's account.</param>
        /// <param name="message">Message of the commit.</param>
        /// <param name="author">Author of the commit. Username if empty.</param>
        /// <param name="folder">Path to a folder in the tree. Root folder if empty.</param>
        /// <param name="fileNames">Names of the files to commit. All files if empty.</param>
        public void Commit(string message, string description = null, string fileName = null, string authorName = null, string authorEmail = null)
        {
            if (!IsCloned())
            {
                throw new Exception("Could not commit, because the repository was not cloned yet.");
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Commit message cannot be empty.");
            }
            
            string descriptionParts = "";
            string temp = "";
            for (int i = 0; description != null && i < description.Length; ++i)
            {
                if (description[i] == '\r' || description[i] == '\n' || i == description.Length - 1)
                {
                    if (temp.Trim().Length > 0)
                    {
                        descriptionParts += string.Format(" -m \"{0}\"", temp.Replace("\"", "\\\""));
                        temp = "";
                    }
                }
                else
                    temp += description[i];
            }
            
            using (var cmd = new CommandPrompt(true))
            {
                cmd.WriteLine("cd {0}", this.Info.Directory);
                cmd.WriteLine("git reset HEAD -- .");
                cmd.WriteLine("git add {0}", !string.IsNullOrEmpty(fileName) ? fileName : ".");
                cmd.WriteLine("git commit{0}{1}{2}",
                    !string.IsNullOrEmpty(authorName) ? string.Format(" --author=\"{0} <{1}>\"", authorName, (!string.IsNullOrEmpty(authorEmail) ? authorEmail : "")) : "",
                    string.Format(" -m \"{0}\"", message), !string.IsNullOrEmpty(descriptionParts) ? descriptionParts : "");
                cmd.WriteLine("git push https://github.com/{0}/{1}/ {2}", this.Info.User, this.Info.Name, this.Info.Branch);
                cmd.WriteLine("exit");

                cmd.WaitForExit();
                cmd.RethrowError();
            }
        }

        /// <summary>
        /// Resets the current repository.
        /// </summary>
        public void Reset()
        {
            if (!IsCloned())
            {
                throw new Exception("Repository has not been cloned yet.");
            }

            _Reset();
        }

        private void _Reset()
        {
            using (var cmd = new CommandPrompt(true))
            {
                cmd.WriteLine("cd {0}", this.Info.Directory);
                cmd.WriteLine("git pull -p");
                cmd.WriteLine("git clean -f -d");
                cmd.WriteLine("exit");

                cmd.RethrowError();
                cmd.WaitForExit();
            }
        }
    }
}
