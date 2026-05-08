Feature: US011 Iniciar sesión como empleado
  Como empleado
  quiero iniciar sesión con mis propias credenciales
  para acceder al sistema

  Scenario Outline: Inicio de sesión exitoso
    Given que el <empleado> tiene una cuenta activa
    When ingresa su <correo> y <contrasena> en el inicio de sesión
    And haga clic en el botón "Iniciar Sesión"
    Then el sistema lo autentica e ingresa a la plataforma.

    Examples:
      | empleado | correo              | contrasena |
      | Carlos   | carlos@empresa.com  | pass123    |

  Scenario Outline: Credenciales incorrectas
    Given que el <empleado> quiere iniciar sesión
    When ingrese un <correo> o <contrasena> incorrectos
    And haga clic en el botón "Iniciar Sesión"
    Then el sistema no permite su acceso a la plataforma y muestra un <mensaje>.

    Examples:
      | empleado | correo            | contrasena | mensaje                |
      | Ana      | ana@empresa.com   | wrongpass  | Credenciales inválidas |
