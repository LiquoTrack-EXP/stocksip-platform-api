using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;

namespace LiquoTrack.StocksipPlatform.AcceptanceTests.Steps;

[Binding]
public class UserLoginSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage _response;

    public UserLoginSteps()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [Given(@"que el (.*) tiene una cuenta activa")]
    public async Task GivenQueElEmpleadoTieneUnaCuentaActiva(string empleado)
    {
        var registrationPayload = new
        {
            email = "carlos@empresa.com",
            password = "pass123",
            name = empleado,
            businessName = "Empresa de Carlos",
            role = "StoreOwner"
        };
        await _client.PostAsJsonAsync("/api/v1/sign-up", registrationPayload);
    }

    [When(@"ingresa su (.*) y (.*) en el inicio de sesión")]
    public void WhenIngresaSuCorreoYContrasenaEnElInicioDeSesion(string correo, string contrasena)
    {
        // Payload capturado para el siguiente paso
        ScenarioContext.Current["LoginPayload"] = new { email = correo, password = contrasena };
    }

    [When(@"haga clic en el botón ""Iniciar Sesión""")]
    public async Task WhenHagaClicEnElBotonIniciarSesion()
    {
        var payload = ScenarioContext.Current["LoginPayload"];
        _response = await _client.PostAsJsonAsync("/api/v1/sign-in", payload);
    }

    [Then(@"el sistema lo autentica e ingresa a la plataforma.")]
    public void ThenElSistemaLoAutenticaEIngresaALaPlataforma()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Given(@"que el (.*) quiere iniciar sesión")]
    public void GivenQueElEmpleadoQuiereIniciarSesion(string empleado)
    {
        // Ready
    }

    [When(@"ingrese un (.*) o (.*) incorrectos")]
    public void WhenIngreseUnCorreoOContrasenaIncorrectos(string correo, string contrasena)
    {
        ScenarioContext.Current["LoginPayload"] = new { email = correo, password = contrasena };
    }

    [Then(@"el sistema no permite su acceso a la plataforma y muestra un (.*).")]
    public void ThenElSistemaNoPermiteSuAccesoALaPlataformaYMuestraUnMensaje(string mensaje)
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
