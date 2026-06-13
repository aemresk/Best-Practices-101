public record EmailNotification(string To, string Subject, string Body);
public record SmsNotification(string PhoneNumber, string Message);
public record PushNotification(string DeviceToken, string Title, string Body);
