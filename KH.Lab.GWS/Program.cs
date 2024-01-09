// See https://aka.ms/new-console-template for more information

using KH.Lab.GWS;

Console.WriteLine("Hello, World!");

var service = new UserService();

string operation = args[0].ToLower();

if (operation == "-l")
{
    service.listUser();
}
else if (operation == "-a")
{
    service.CreateUser();
}
else if (operation == "-u")
{
    service.UpdateUser();
}
else if (operation == "-d")
{
    service.DeleteUser();
}
else
{
    Console.WriteLine("參數錯誤。 請指定 [-l]、[-a]、[-u] 或 [d]。");
    return;
}
return;