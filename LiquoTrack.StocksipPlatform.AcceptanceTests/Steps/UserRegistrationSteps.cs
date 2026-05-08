using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;

namespace LiquoTrack.StocksipPlatform.AcceptanceTests.Steps;

[Binding]
public class UserRegistrationSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage _response;
    private object _registrationPayload;

    public UserRegistrationSteps()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [Given(@"el (.*) no posee una cuenta")]
    public void GivenElVisitanteNoPoseeUnaCuenta(string visitante)
    {
        //Ready for registration
    }

    [When(@"complete su registro ingresando su (.*), (.*), (.*) y (.*)")]
    public void WhenCompleteSuRegistroIngresandoSuNombreEmpresaCorreoRolYContrasena(string nombreEmpresa, string correo, string rol, string contrasena)
    {
        _registrationPayload = new
        {
            email = correo,
            password = contrasena,
            name = "Nombre del Visitante",
            businessName = nombreEmpresa,
            role = rol
        };
    }

    [When(@"haga clic en el botón ""Registrarse""")]
    public async Task WhenHagaClicEnElBotonRegistrarse()
    {
        _response = await _client.PostAsJsonAsync("/api/v1/sign-up", _registrationPayload);
    }

    [Then(@"el sistema crea una cuenta para dicho visitante.")]
    public void ThenElSistemaCreaUnaCuentaParaDichoVisitante()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [When(@"intenta registrarse pero no completa el campo de (.*)")]
    public void WhenIntentaRegistrarsePeroNoCompletaElCampoDeInformacionFaltante(string informacionFaltante)
    {
        _registrationPayload = new
        {
            email = informacionFaltante == "correo" ? "" : "valid@email.com",
            password = informacionFaltante == "contraseña" ? "" : "validPass123",
            name = "Missing Info User",
            businessName = "Test Business",
            role = "StoreOwner"
        };
    }

    [Then(@"el sistema le mostrará un (.*).")]
    public void ThenElSistemaLeMostraraUnMensajeDeError(string mensajeDeError)
    {
        _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
