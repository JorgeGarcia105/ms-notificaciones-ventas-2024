using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notificaciones.Models;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon;

namespace ms_notificaciones.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificacionesController : ControllerBase
{
    /**[Route("correo-bienvenida")]
    [HttpPost]
    public async Task<IActionResult> EnviarCorreoBienvenida( ModeloCorreo datos) {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new {
            name=datos.nombreDestino,
            message="Bienvenido a la comunidad de la inmobiliria"
        });
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted) {
            return Ok("Correo enviado correctamente");
        } else {
            return BadRequest("Error al enviar el correo" + datos.correoDestino);
        }
    }


    [Route("correo-recuperacion-clave")]
    [HttpPost]
    public async Task<IActionResult> EnviarCorreoRecuperacionClave( ModeloCorreo datos) {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new {
            name=datos.nombreDestino,
            message="Esta es tu nueva clave.... no la comparta."
        });
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted) {
            return Ok("Correo enviado a la direccion" + datos.contenidoCorreo);
        } else {
            return BadRequest("Error al enviar el correo" + datos.correoDestino);
        }
    }

    [Route("enviar-correo-2fa")]
    [HttpPost]
    public async Task<IActionResult> EnviarCorreo2fa( ModeloCorreo datos) {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CrearMensajeBase(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("TwoFA_SENGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new {
            nombre=datos.nombreDestino,
            mensaje=datos.contenidoCorreo,
            asunto=datos.asuntoCorreo
        });
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted) {
            return Ok("Correo enviado correctamente");
        } else {
            return BadRequest("Error al enviar el correo" + datos.correoDestino);
        }
    }

    private SendGridMessage CrearMensajeBase(ModeloCorreo datos) {
        
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = datos.asuntoCorreo;
        var to = new EmailAddress(datos.correoDestino, datos.nombreDestino);
        var plainTextContent = datos.contenidoCorreo;
        var htmlContent = datos.contenidoCorreo;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENGRID_TEMPLATE_ID"));
        return msg;
    }*/

    // Envio de sms
    [Route("enviar-sms")]
    [HttpPost]
    public async Task<ActionResult> EnviarSMSNuevaClave(ModeloSms datos) {

        var accesskey = Environment.GetEnvironmentVariable("ACCESS_KEY_AWS");
        var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY_AWS");
        var client = new AmazonSimpleNotificationServiceClient(accesskey, secretKey, RegionEndpoint.USEast1);
        var messageAtributes = new Dictionary<string, MessageAttributeValue>();
        var smsType = new MessageAttributeValue {
            DataType = "String",
            StringValue = "Transactional"
        };

        messageAtributes.Add("AWS.SNS.SMS.SMSType", smsType);

        PublishRequest request = new PublishRequest {
            Message = datos.contenidoMensaje,
            PhoneNumber = datos.numeroDestino,
            MessageAttributes = messageAtributes
        };

        try {
            await client.PublishAsync(request);
            return Ok("SMS enviado correctamente");
        } catch{
            return BadRequest("Error al enviar el sms");
        }
    }

}
