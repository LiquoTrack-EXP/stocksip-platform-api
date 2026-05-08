Feature: US001 Registro de usuarios en la plataforma
  Como visitante
  quiero crear una cuenta en la plataforma
  para utilizar las funcionalidades de gestión de inventario

  Scenario Outline: Visitante realiza un registro exitoso
    Given el <visitante> no posee una cuenta
    When complete su registro ingresando su <nombre_empresa>, <correo>, <rol> y <contraseña>
    And haga clic en el botón "Registrarse"
    Then el sistema crea una cuenta para dicho visitante.

    Examples:
      | visitante | nombre_empresa  | correo            | rol        | contrasena        |
      | Juan      | Licorería El Sol| juan@example.com  | StoreOwner | SecurePassword123 |

  Scenario Outline: Visitante falla al realizar su registro
    Given el <visitante> no posee una cuenta
    When intenta registrarse pero no completa el campo de <informacion_faltante>
    And haga clic en el botón "Registrarse"
    Then el sistema le mostrará un <mensaje_de_error>.

    Examples:
      | visitante | informacion_faltante | mensaje_de_error                     |
      | Pedro     | correo               | Faltan completar campos obligatorios |
      | Maria     | contraseña           | Faltan completar campos obligatorios |
