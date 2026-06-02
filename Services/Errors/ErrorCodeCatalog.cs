namespace frontendnet.Services.Errors;

public static class ErrorCodeCatalog
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string EmailNotConfirmed = "EMAIL_NOT_CONFIRMED";
    public const string AuthSessionExpired = "AUTH_SESSION_EXPIRED";
    public const string Forbidden = "FORBIDDEN";
    public const string AuthTokenInvalid = "AUTH_TOKEN_INVALID";
    public const string PasswordCurrentInvalid = "PASSWORD_CURRENT_INVALID";
    public const string PasswordWeak = "PASSWORD_WEAK";
    public const string PasswordConfirmationMismatch = "PASSWORD_CONFIRMATION_MISMATCH";
    public const string PasswordReuse = "PASSWORD_REUSE";
    public const string EmailAlreadyRegistered = "EMAIL_ALREADY_REGISTERED";
    public const string UserHasAssociatedOrders = "USER_HAS_ASSOCIATED_ORDERS";
    public const string EmailVerificationRequired = "EMAIL_VERIFICATION_REQUIRED";
    public const string EmailVerificationCodeInvalid = "EMAIL_VERIFICATION_CODE_INVALID";
    public const string EmailVerificationCodeExpired = "EMAIL_VERIFICATION_CODE_EXPIRED";
    public const string EmailSendFailed = "EMAIL_SEND_FAILED";
    public const string PasswordChangeCodeInvalid = "PASSWORD_CHANGE_CODE_INVALID";
    public const string PasswordChangeCodeExpired = "PASSWORD_CHANGE_CODE_EXPIRED";
    public const string PasswordChangeCodeRateLimited = "PASSWORD_CHANGE_CODE_RATE_LIMITED";
    public const string PasswordChanged = "PASSWORD_CHANGED";
    public const string SessionClosed = "SESSION_CLOSED";
    public const string FileNotFound = "FILE_NOT_FOUND";
    public const string FileInvalidType = "FILE_INVALID_TYPE";
    public const string FileTooLarge = "FILE_TOO_LARGE";
    public const string FileUploadFailed = "FILE_UPLOAD_FAILED";
    public const string ImageNotAvailable = "IMAGE_NOT_AVAILABLE";
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
    public const string ProductNotFound = "PRODUCT_NOT_FOUND";
    public const string OrderInvalidStatus = "ORDER_INVALID_STATUS";
    public const string ServerUnavailable = "SERVER_UNAVAILABLE";
    public const string InternalError = "INTERNAL_ERROR";
}
