using System.IO;
using MimeKit;
using Octokit.Helpers;
using Windows.ApplicationModel.Email;
using Windows.System;

namespace BlOrders2023.Helpers;

public static class Helpers
{
    public static async Task SendEmailAsync(string email)
    {
        await Launcher.LaunchUriAsync(new Uri(String.Format("mailto:{0}", email)));
    }

    public static string SendEmailAsync(string to, string subject, string body, IEnumerable<string> attachemntPath)
    {
        var TempPath = Path.GetTempPath() + "BLOrders2023";
        var emlPath = TempPath + Path.PathSeparator + DateTime.UtcNow.ToFileTimeUtc() +".eml";
        // Create a new MIME message
        var message = new MimeMessage();

        // Add sender and recipient
        message.To.Add(new MailboxAddress(to,to));
        message.Subject = subject;
        var builder = new BodyBuilder();
        builder.TextBody = body;
        
        foreach (var item in attachemntPath)
        {
            builder.Attachments.Add(item);
        }
        
        message.Body = builder.ToMessageBody();
        // Generate the .eml file
        
        using (var stream = File.Create(emlPath))
        {
            var binaryWriter = new BinaryWriter(stream);
            //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
            binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));
            message.WriteTo(stream);
        }

        LauncherOptions options = new()
        {

        };
        _ = Launcher.LaunchUriAsync(new Uri(emlPath), options);

        return emlPath;
    }
}