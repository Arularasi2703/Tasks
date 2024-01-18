namespace FoodOrderingSystemAPI.Models{
  public class EmailSettings
{
    public string fromMail { get; set; }
    public string fromPassword { get; set; }
    public string smtpHost { get; set; }
    public int smtpPort { get; set; }
    public string otpEmailTemplate { get; set; }
    public string otpEmailSubject { get; set; }
}

}