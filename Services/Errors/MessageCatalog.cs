namespace frontendnet.Services.Errors;

public static class MessageCatalog
{
    public const string ServerUnavailable = "El servidor no esta disponible en este momento. Intentalo mas tarde.";
    public const string SessionExpired = "Tu sesion expiro. Inicia sesion nuevamente.";
    public const string Forbidden = "No tienes permisos para realizar esta accion.";
    public const string Validation = "Revisa los datos ingresados.";
    public const string Generic = "No fue posible procesar la solicitud. Intentalo nuevamente.";

    private static readonly IReadOnlyDictionary<string, string> Messages = new Dictionary<string, string>
    {
        [ErrorCodeCatalog.ValidationError] = Validation,
        [ErrorCodeCatalog.AuthInvalidCredentials] = "Usuario o contrasena incorrectos.",
        [ErrorCodeCatalog.AuthEmailNotConfirmed] = "Debes confirmar tu correo electronico antes de iniciar sesion.",
        [ErrorCodeCatalog.AuthSessionExpired] = SessionExpired,
        [ErrorCodeCatalog.AuthForbidden] = Forbidden,
        [ErrorCodeCatalog.PasswordCurrentInvalid] = "La contrasena actual no es correcta.",
        [ErrorCodeCatalog.PasswordWeak] = "La contrasena no cumple la politica de seguridad.",
        [ErrorCodeCatalog.PasswordConfirmationMismatch] = "La nueva contrasena y la confirmacion no coinciden.",
        [ErrorCodeCatalog.PasswordReuse] = "La nueva contrasena debe ser diferente a la actual.",
        [ErrorCodeCatalog.EmailAlreadyRegistered] = "El correo electronico ya esta registrado.",
        [ErrorCodeCatalog.UserHasAssociatedOrders] = "No se puede eliminar el usuario porque tiene pedidos asociados.",
        [ErrorCodeCatalog.EmailConfirmationInvalid] = "El codigo de confirmacion es invalido o expiro.",
        [ErrorCodeCatalog.EmailSendFailed] = "No fue posible enviar el correo. Intentalo nuevamente mas tarde.",
        [ErrorCodeCatalog.PasswordChangeCodeInvalid] = "El codigo no es valido o ha expirado.",
        [ErrorCodeCatalog.PasswordChangeCodeExpired] = "El codigo no es valido o ha expirado.",
        [ErrorCodeCatalog.PasswordChangeCodeRateLimited] = "Espera antes de solicitar otro codigo.",
        [ErrorCodeCatalog.PasswordChanged] = "Contrasena actualizada correctamente.",
        [ErrorCodeCatalog.SessionClosed] = "Tu sesion se cerro correctamente.",
        [ErrorCodeCatalog.FileInvalidType] = "El archivo no es una imagen permitida.",
        [ErrorCodeCatalog.FileUploadFailed] = "No fue posible guardar el archivo.",
        [ErrorCodeCatalog.ImageNotAvailable] = "La imagen no esta disponible.",
        [ErrorCodeCatalog.ResourceNotFound] = "El recurso solicitado no existe.",
        [ErrorCodeCatalog.ProductNotFound] = "El producto solicitado no esta disponible.",
        [ErrorCodeCatalog.OrderInvalidStatus] = "El estado del pedido no es valido.",
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
