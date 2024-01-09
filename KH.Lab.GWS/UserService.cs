using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace KH.Lab.GWS
{
    internal class UserService
    {
        static string[] Scopes = { DirectoryService.Scope.AdminDirectoryUserReadonly, DirectoryService.Scope.AdminDirectoryUser };

        static string domainName = "presco.com.tw";

        static UserCredential credential;

        public UserService()
        {
            string credFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cred.json");
            using (var stream = new FileStream(credFile, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
        }

        // 查詢
        public void listUser()
        {
            // Create Directory API service.
            var service = new DirectoryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });

            string nextPageToken = string.Empty;
            do
            {
                // Define parameters of request.
                UsersResource.ListRequest request = service.Users.List();
                request.MaxResults = 1;
                request.Domain = domainName;
                request.PageToken = nextPageToken;
                request.OrderBy = UsersResource.ListRequest.OrderByEnum.Email;

                // List users.
                var ret = request.Execute();
                var users = ret.UsersValue;
                if (users != null && users.Count > 0)
                {
                    foreach (var userItem in users)
                    {
                        Console.WriteLine("{0} ({1})", userItem.PrimaryEmail, userItem.Name.FullName);
                    }
                }
                else
                {
                    Console.WriteLine("No users found.");
                    break;
                }

                if (!string.IsNullOrEmpty(ret.NextPageToken))
                {
                    nextPageToken = ret.NextPageToken;
                }
                else
                {
                    nextPageToken = string.Empty;
                }

            } while (!string.IsNullOrEmpty(nextPageToken));
        }

        // 新增
        public void CreateUser()
        {
            try
            {
                // Create Directory API service.
                var service = new DirectoryService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                });

                // Define parameters of request.
                User uParam = new User();
                uParam.Name = new UserName();

                Console.WriteLine("請輸入您的姓：");
                uParam.Name.FamilyName = Console.ReadLine();
                Console.WriteLine("請輸入您的名：");
                uParam.Name.GivenName = Console.ReadLine();
                Console.WriteLine("請輸入您的初始密碼：");
                uParam.Password = Console.ReadLine();
                Console.WriteLine("請輸入帳號：");
                uParam.PrimaryEmail = Console.ReadLine() + @"@" + domainName;

                UsersResource.InsertRequest request = service.Users.Insert(uParam);
                User ret = request.Execute();

                Console.WriteLine("[{0}]新增成功。", ret.PrimaryEmail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("建立失敗。[{0}]", ex.ToString());
            }
        }

        // 更新
        public void UpdateUser()
        {
            try
            {
                // Create Directory API service.
                var service = new DirectoryService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                });

                Console.WriteLine("請輸入要更新的帳號：");
                string userKey = Console.ReadLine() + @"@" + domainName;

                // Define parameters of request.
                User uParam = new User();
                uParam.Name = new UserName();

                Console.WriteLine("變更的姓:");
                uParam.Name.FamilyName = Console.ReadLine();
                Console.WriteLine("變更的名:");
                uParam.Name.GivenName = Console.ReadLine();

                UsersResource.UpdateRequest request = service.Users.Update(uParam, userKey);
                User ret = request.Execute();

                Console.WriteLine("{0}更新成功。", userKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("更新失敗。[{0}]", ex.ToString());
            }
        }

        // 刪除
        public void DeleteUser()
        {
            try
            {
                // Create Directory API service.
                var service = new DirectoryService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                });

                Console.WriteLine("請輸入要刪除的帳號:");
                string userKey = Console.ReadLine() + @"@" + domainName;

                UsersResource.DeleteRequest request = service.Users.Delete(userKey);
                var ret = request.Execute();

                Console.WriteLine("{0}刪除成功。", userKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("刪除失敗。[{0}]", ex.ToString());
            }
        }
    }
}
