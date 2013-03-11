using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace bug_in_MailMessage_quoted_printable
{
    class Program
    {
        static void Main(string[] args){

            var address = "gh-20121204@hotmail.com";
            var mailSubject = "Causes invalid dot-stuffing with quoted printable";
            var mailBody = File.ReadAllText("./body.htm"); // this contains some obfuscated "HTML" that produces the issue

            // uses ASCII and quoted-printable by default (I hope)
            var mailMessage = new MailMessage(address, address, "", mailBody);

            mailMessage.Save("result.eml");

        }

    }

    // hack to save the content of the mail message to a file from:
    // http://www.codeproject.com/Articles/32434/Adding-Save-functionality-to-Microsoft-Net-Mail-Ma
    // Modified for framework differences
    public static class MailMessageExt
    {
        public static void Save(this MailMessage Message, string FileName) {
            Assembly assembly = typeof(SmtpClient).Assembly;
            Type _mailWriterType =
              assembly.GetType("System.Net.Mail.MailWriter");

            using (FileStream _fileStream =
                   new FileStream(FileName, FileMode.Create)) {
                // Get reflection info for MailWriter contructor
                ConstructorInfo _mailWriterContructor =
                    _mailWriterType.GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new Type[] { typeof(Stream) },
                        null);

                // Construct MailWriter object with our FileStream
                object _mailWriter =
                  _mailWriterContructor.Invoke(new object[] { _fileStream });

                // Get reflection info for Send() method on MailMessage
                MethodInfo _sendMethod =
                    typeof(MailMessage).GetMethod(
                        "Send",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                // Call method passing in MailWriter
                try{
                    _sendMethod.Invoke(
                        Message,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new object[]{_mailWriter, true},
                        null);
                }
                catch (TargetParameterCountException parameterCountException){
                    Console.WriteLine("This version of the framework will probably not produce the issue.");
                    Console.WriteLine("Press the [Any] key to continue.");
                    Console.ReadKey();
                    _sendMethod.Invoke(
                        Message,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new object[] { _mailWriter, true, false },
                        null);
                }

                // Finally get reflection info for Close() method on our MailWriter
                MethodInfo _closeMethod =
                    _mailWriter.GetType().GetMethod(
                        "Close",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                _closeMethod.Invoke(
                    _mailWriter,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { },
                    null);
            }
        }
    }

}
