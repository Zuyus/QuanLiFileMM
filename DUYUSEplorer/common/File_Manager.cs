using Microsoft.AspNetCore.Mvc;

namespace NDExplorer.common
{
    public class File_Manager
    {
        protected string _rootPath;
        protected string _command;
        protected string _value;
        // Dùng trong trường hợp đổi tên , di chuyển file.
        protected string _secondaryValue;
        public File_Manager(string rootPath, HttpRequest request)
        {
            _rootPath = rootPath;
            _command = request.Query["cmd"].ToString();
            _value = request.Query["value"].ToString();
            _secondaryValue = request.Query["secondaryValue"].ToString();

        }

        public FileManagerResponse ExecuteCmd()
        {
            FileManagerResponse response = new();
            try
            {
                switch (_command)
                {
                    case "GET_ALL_DIR":
                        {
                            response.Data = GetAllDirs();
                            break;
                        }

                        // Lấy file trong thư mục khác
                    case "GET_ALL_IN_DIR":
                        {
                            response.Data = GetAllIndir(_value);
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return response;
        }
        protected List<FMFolderItem> GetAllIndir(string folder)
        {
            var result = new List<FMFolderItem>();
            var fullPath = Path.Combine(_rootPath, folder);
            var dirs = Directory.GetDirectories(fullPath)
                    .Select(d => new FMFolderItem
                    {
                        Path = d.Replace(_rootPath + "\\", ""),
                        Name = Path.GetFileName(d), // Lấy tên thư mục 
                        IsFolder = true
                    });

            var files = Directory.GetFiles(fullPath)
                   .Select(f => new FMFolderItem
                   {
                       Path = f.Replace(_rootPath + "\\", ""),
                       Name = f.Split("\\")[^1], 
                       IsFolder = false
                   });

            result.AddRange(dirs);
            result.AddRange(files);

            return result;
        }

        protected string[] GetAllDirs()
        {
            var dirs = Directory.GetDirectories(_rootPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < dirs.Length; i++)
            {
                dirs[i] = dirs[i].Replace(_rootPath, string.Empty).Trim(Path.DirectorySeparatorChar); // Ký tự phân cách thư mục 'Lấy dấu đường dẫn tùy vào HĐH'
            }
            return dirs.OrderBy(d => d).ToArray();
        }
    }

    public class FileManagerResponse
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    public class FMFolderItem
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsFolder { get; set; }
    }
}
