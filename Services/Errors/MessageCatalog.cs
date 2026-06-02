namespace frontendnet.Services.Errors;

public static class MessageCatalog
{
    public const string ServerUnavailable = "El servidor no está disponible en este momento. Inténtalo más tarde.";
    public const string SessionExpired = "Tu sesión expiró. Inicia sesión nuevamente.";
    public const string Forbidden = "No tienes permisos para realizar esta acción.";
    public const string Validation = "Revisa los datos ingresados.";
    public const string Generic = "No fue posible procesar la solicitud. Inténtalo nuevamente.";

    private static readonly IReadOnlyDictionary<string, string> Messages = new Dictionary<string, string>
    {
        [ErrorCodeCatalog.ValidationError] = Validation,
        [ErrorCodeCatalog.InvalidCredentials] = "Credenciales incorrectas.",
        [ErrorCodeCatalog.UserNotFound] = "Para acceder al sistema primero debes registrarte.",
        [ErrorCodeCatalog.EmailNotConfirmed] = "Debes confirmar tu correo electrónico antes de iniciar sesión.",
        [ErrorCodeCatalog.AuthSessionExpired] = SessionExpired,
        [ErrorCodeCatalog.AuthTokenInvalid] = SessionExpired,
        [ErrorCodeCatalog.Forbidden] = Forbidden,
        [ErrorCodeCatalog.PasswordCurrentInvalid] = "La contraseña actual no es correcta.",
        [ErrorCodeCatalog.PasswordWeak] = "La contraseña no cumple la política de seguridad.",
        [ErrorCodeCatalog.PasswordConfirmationMismatch] = "La nueva contraseña y la confirmación no coinciden.",
        [ErrorCodeCatalog.PasswordReuse] = "La nueva contraseña debe ser diferente a la actual.",
        [ErrorCodeCatalog.EmailAlreadyRegistered] = "El correo electrónico ya está registrado.",
        [ErrorCodeCatalog.UserHasAssociatedOrders] = "No se puede eliminar el usuario porque tiene pedidos asociados.",
        [ErrorCodeCatalog.EmailVerificationRequired] = "No hay una verificación de correo pendiente. Regístrate nuevamente.",
        [ErrorCodeCatalog.EmailVerificationCodeInvalid] = "El código de verificación no es válido.",
        [ErrorCodeCatalog.EmailVerificationCodeExpired] = "El código de verificación expiró. Solicita uno nuevo.",
        [ErrorCodeCatalog.EmailSendFailed] = "No fue posible enviar el correo. Inténtalo nuevamente más tarde.",
        [ErrorCodeCatalog.PasswordChangeCodeInvalid] = "El código no es válido o ha expirado.",
        [ErrorCodeCatalog.PasswordChangeCodeExpired] = "El código no es válido o ha expirado.",
        [ErrorCodeCatalog.PasswordChangeCodeRateLimited] = "Espera antes de solicitar otro código.",
        [ErrorCodeCatalog.PasswordChanged] = "Contraseña actualizada correctamente.",
        [ErrorCodeCatalog.SessionClosed] = "Tu sesión se cerró correctamente.",
        [ErrorCodeCatalog.FileNotFound] = "El archivo solicitado no está disponible.",
        [ErrorCodeCatalog.FileInvalidType] = "El archivo no es una imagen permitida.",
        [ErrorCodeCatalog.FileTooLarge] = "El archivo supera el tamaño máximo permitido.",
        [ErrorCodeCatalog.FileUploadFailed] = "No fue posible guardar el archivo.",
        [ErrorCodeCatalog.ImageNotAvailable] = "La imagen no está disponible.",
        [ErrorCodeCatalog.ResourceNotFound] = "El recurso solicitado no existe.",
        [ErrorCodeCatalog.ProductNotFound] = "El producto solicitado no está disponible.",
        [ErrorCodeCatalog.OrderInvalidStatus] = "El estado del pedido no es válido.",
        [ErrorCodeCatalog.ServerUnavailable] = ServerUnavailable,
        [ErrorCodeCatalog.InternalError] = Generic,
    };

    public static string GetMessage(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && Messages.TryGetValue(code, out var message)
            ? message
            : Generic;
    }
}
