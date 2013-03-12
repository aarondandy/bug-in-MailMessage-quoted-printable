Bug in MailMessage with quoted-printable
================================

A bug in .NET MailMessage: Invalid dot-stuffing when using quoted-printable encoding.

Description
-----------

SMTP does not allow for leading '.' characters on a line, so they are to be escaped as ".." (dot-stuffing) for transfer (I think). Most of the time MailMessage does this for quoted-printable messages. I found an input that causes a leading '.' to sometimes be escaped as **three** dots. We also have encountered situations (not reproducible, yet) where we think '.' characters are removed from the message, probably due to errors in escaping.

* This appears to be fixed on machines with .NET 4.5 installed.
* It seems that the bug can be determined by a private reflection "check": [Program.cs#L68](https://github.com/aarondandy/bug-in-MailMessage-quoted-printable/blob/master/Program.cs#L68 "Program.cs#L68")
* I am not offered any .NET updates on my .NET 4.0 machines.

Issue: [781115](https://connect.microsoft.com/VisualStudio/feedback/details/781115/invalid-dot-stuffing-for-quoted-printable-mailmessage)