﻿namespace PustokTask.Settings;

public class EmailSetting
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public string EnableSsl { get; set; }
}
