# 📋 USER STORIES - VisitApp (Organizadas por Rol)
## Sistema de Gestión de Visitas Pastorales IASD

### 🎯 CRITERIOS INVEST APLICADOS
- **I**ndependent: Cada historia es independiente y puede desarrollarse por separado
- **N**egotiable: Los detalles pueden ajustarse según necesidades del negocio
- **V**aluable: Cada historia aporta valor directo al usuario final
- **E**stimable: Historias con complejidad clara y estimable
- **S**mall: Tamaño apropiado para completar en un sprint
- **T**estable: Criterios de aceptación verificables

---


## 🛡️ HU de Administrador

### HU-ADM-01: Gestión de roles y asignación
**Como** administrador
**Quiero** ver y asignar roles a los usuarios registrados
**Para** controlar el acceso y las funcionalidades de la aplicación


## 🧑‍🏫 HU de Líder

### HU-LID-01: Supervisión de familias
**Como** líder
**Quiero** visualizar el avance y estado de las familias asignadas
**Para** dar seguimiento y apoyo personalizado

**Criterios de aceptación:**
```gherkin

## 👨‍👩‍👧‍👦 HU de Familia

### HU-FAM-01: Gestión de conceptos (contactos y notas)
**Como** familia
**Quiero** crear, editar, eliminar y visualizar mis contactos y notas
**Para** organizar la información relevante de mis visitas y relaciones

**Criterios de aceptación:**
```gherkin
Feature: CRUD de conceptos de familia
  Como familia
  Quiero gestionar mis contactos y notas
  Para organizar información relevante

  Scenario: Crear contacto o nota
    Given que accedo al módulo de conceptos
    When creo un nuevo contacto o nota
    Then la información se guarda correctamente y es visible solo para mi familia

  Scenario: Editar o eliminar contacto o nota
    Given que selecciono un contacto o nota existente
    When edito o elimino
    Then los cambios se reflejan correctamente
```

### HU-FAM-02: Visualización y edición de visitas
**Como** familia
**Quiero** visualizar y editar mis visitas programadas y completadas
**Para** llevar un control de mi historial y próximos compromisos

**Criterios de aceptación:**
```gherkin
Feature: Visualización y edición de visitas de familia
  Como familia
  Quiero ver y editar mis visitas
  Para llevar control de mi historial y próximos compromisos

  Scenario: Ver visitas programadas y completadas
    Given que accedo al módulo de visitas
    When consulto la lista
    Then veo mis visitas programadas y el historial de completadas

  Scenario: Editar visita
    Given que selecciono una visita
    When modifico los datos y guardo
    Then la información se actualiza correctamente solo para mi familia
```

### HU-FAM-03: Visualización y descarga de PDF de temas bíblicos
**Como** familia
**Quiero** visualizar y descargar archivos PDF de temas bíblicos
**Para** estudiar y compartir en familia

**Criterios de aceptación:**
```gherkin
Feature: Visualización y descarga de PDF de temas (familia)
  Como familia
  Quiero ver y descargar PDF de temas bíblicos
  Para estudiar y compartir en familia

  Scenario: Visualizar PDF de tema
    Given que accedo a la sección de temas
    When selecciono un tema con PDF
    Then puedo visualizar el archivo en la app

  Scenario: Descargar PDF de tema
    Given que accedo a la sección de temas
    When selecciono un tema con PDF
    Then puedo descargar el archivo a mi dispositivo
```

### HU-FAM-04: Visualización de preguntas clave
**Como** familia
**Quiero** visualizar las preguntas clave disponibles en el sistema
**Para** utilizarlas en el estudio y reflexión familiar

**Criterios de aceptación:**
```gherkin
Feature: Visualización de preguntas clave (familia)
  Como familia
  Quiero ver preguntas clave
  Para utilizarlas en el estudio familiar

  Scenario: Ver listado de preguntas clave
    Given que accedo a la sección de preguntas clave
    Then veo la lista completa y actualizada de preguntas
```

### HU-FAM-05: Gestión de perfil
**Como** familia
**Quiero** editar y actualizar mi perfil (nombre, email, teléfono, contraseña)
**Para** mantener mi información personal actualizada y segura

**Criterios de aceptación:**
```gherkin
Feature: Edición de perfil de familia
  Como familia
  Quiero editar mi perfil
  Para mantener mi información actualizada

  Scenario: Editar y guardar cambios
    Given que accedo a la sección de perfil
    When modifico mis datos y guardo los cambios
    Then la información se actualiza correctamente y recibo confirmación
```

### HU-FAM-06: Experiencia multidispositivo de familia
**Como** familia
**Quiero** usar la aplicación en diferentes dispositivos y en modo offline básico
**Para** acceder a la información y participar desde cualquier lugar

**Criterios de aceptación:**
```gherkin
Feature: Uso multidispositivo y offline de familia
  Como familia
  Quiero usar la app en móvil, tablet o desktop
  Para acceder y participar desde cualquier lugar

  Scenario: Sincronización entre dispositivos
    Given que uso la app en diferentes dispositivos
    When realizo una acción familiar
    Then la información se actualiza en todos los dispositivos

  Scenario: Modo offline básico
    Given que estoy sin conexión
    When accedo a datos guardados
    Then puedo ver información básica y tomar notas que se sincronizan al recuperar conexión
```
    Given que accedo a la sección de temas
    When selecciono un tema con PDF
    Then puedo descargar el archivo a mi dispositivo
```

### HU-LID-05: Visualización de preguntas clave
**Como** líder
**Quiero** visualizar las preguntas clave disponibles en el sistema
**Para** utilizarlas en visitas y estudios

**Criterios de aceptación:**
```gherkin
Feature: Visualización de preguntas clave (líder)
  Como líder
  Quiero ver preguntas clave
  Para utilizarlas en visitas y estudios

  Scenario: Ver listado de preguntas clave
    Given que accedo a la sección de preguntas clave
    Then veo la lista completa y actualizada de preguntas
```

### HU-LID-06: Gestión de perfil
**Como** líder
**Quiero** editar y actualizar mi perfil (nombre, email, teléfono, contraseña)
**Para** mantener mi información personal actualizada y segura

**Criterios de aceptación:**
```gherkin
Feature: Edición de perfil de líder
  Como líder
  Quiero editar mi perfil
  Para mantener mi información actualizada

  Scenario: Editar y guardar cambios
    Given que accedo a la sección de perfil
    When modifico mis datos y guardo los cambios
    Then la información se actualiza correctamente y recibo confirmación
```

### HU-LID-07: Experiencia multidispositivo de líder
**Como** líder
**Quiero** usar la aplicación en diferentes dispositivos y en modo offline básico
**Para** gestionar y dar seguimiento desde cualquier lugar

**Criterios de aceptación:**
```gherkin
Feature: Uso multidispositivo y offline de líder
  Como líder
  Quiero usar la app en móvil, tablet o desktop
  Para gestionar y dar seguimiento desde cualquier lugar

  Scenario: Sincronización entre dispositivos
    Given que uso la app en diferentes dispositivos
    When realizo una acción de gestión
    Then la información se actualiza en todos los dispositivos

  Scenario: Modo offline básico
    Given que estoy sin conexión
    When accedo a datos guardados
    Then puedo ver información básica y tomar notas que se sincronizan al recuperar conexión
```
        Given que selecciono un contacto
        When modifico los datos y guardo
        Then los cambios se reflejan correctamente

      Scenario: Eliminar contacto
        Given que selecciono un contacto
        When presiono "Eliminar"
        Then el contacto se elimina tras confirmación
    ```

    ### HU-PAS-03: Gestión de visitas pastorales
    **Como** pastor
    **Quiero** programar, completar y registrar observaciones de visitas pastorales
    **Para** mantener un historial detallado de mi ministerio

    **Criterios de aceptación:**
    ```gherkin
    Feature: Programación y registro de visitas
      Como pastor
      Quiero programar y registrar visitas
      Para mantener historial ministerial

      Scenario: Programar visita
        Given que accedo al módulo de visitas
        When selecciono contacto, fecha y hora
        Then la visita se programa correctamente

      Scenario: Completar visita
        Given que tengo una visita programada
        When la realizo y registro observaciones
        Then la visita cambia a estado "Completada" y se guarda el historial

      Scenario: Cancelar visita
        Given que tengo una visita programada
        When la cancelo con motivo
        Then la visita cambia a estado "Cancelada" y se guarda el motivo
    ```

    ### HU-PAS-04: Supervisión de líderes y familias
    **Como** pastor
    **Quiero** visualizar el avance y estado de los líderes y familias bajo mi supervisión
    **Para** dar seguimiento y apoyo oportuno

    **Criterios de aceptación:**
    ```gherkin
    Feature: Supervisión de líderes y familias
      Como pastor
      Quiero ver el avance de líderes y familias
      Para dar seguimiento y apoyo

      Scenario: Visualizar métricas y reportes
        Given que accedo al panel de supervisión
        When consulto el estado de los líderes y familias
        Then veo métricas, reportes y detalles relevantes de cada uno
    ```

    ### HU-PAS-05: Generación de reportes
    **Como** pastor
    **Quiero** generar y exportar reportes de visitas, temas y preguntas clave
    **Para** analizar el desempeño y compartir información con la administración

    **Criterios de aceptación:**
    ```gherkin
    Feature: Generación de reportes
      Como pastor
      Quiero generar reportes
      Para analizar desempeño y compartir información

      Scenario: Generar reporte por fechas o distrito
        Given que accedo al módulo de reportes
        When selecciono rango de fechas o distrito
        Then obtengo un archivo descargable con los datos solicitados
    ```

    ### HU-PAS-06: Visualización y descarga de PDF de temas bíblicos
    **Como** pastor
    **Quiero** visualizar y descargar archivos PDF de temas bíblicos
    **Para** preparar y compartir estudios con mis contactos

    **Criterios de aceptación:**
    ```gherkin
    Feature: Visualización y descarga de PDF de temas
      Como pastor
      Quiero ver y descargar PDF de temas bíblicos
      Para preparar y compartir estudios

      Scenario: Visualizar PDF de tema
        Given que accedo a la sección de temas
        When selecciono un tema con PDF
        Then puedo visualizar el archivo en la app

      Scenario: Descargar PDF de tema
        Given que accedo a la sección de temas
        When selecciono un tema con PDF
        Then puedo descargar el archivo a mi dispositivo
    ```

    ### HU-PAS-07: Visualización de preguntas clave
    **Como** pastor
    **Quiero** visualizar las preguntas clave disponibles en el sistema
    **Para** utilizarlas en mis visitas y estudios

    **Criterios de aceptación:**
    ```gherkin
    Feature: Visualización de preguntas clave
      Como pastor
      Quiero ver preguntas clave
      Para utilizarlas en visitas y estudios

      Scenario: Ver listado de preguntas clave
        Given que accedo a la sección de preguntas clave
        Then veo la lista completa y actualizada de preguntas
    ```

    ### HU-PAS-08: Gestión de perfil
    **Como** pastor
    **Quiero** editar y actualizar mi perfil (nombre, email, teléfono, contraseña)
    **Para** mantener mi información personal actualizada y segura

    **Criterios de aceptación:**
    ```gherkin
    Feature: Edición de perfil de pastor
      Como pastor
      Quiero editar mi perfil
      Para mantener mi información actualizada

      Scenario: Editar y guardar cambios
        Given que accedo a la sección de perfil
        When modifico mis datos y guardo los cambios
        Then la información se actualiza correctamente y recibo confirmación
    ```

    ### HU-PAS-09: Notificaciones y recordatorios
    **Como** pastor
    **Quiero** recibir notificaciones y recordatorios de visitas programadas y seguimientos
    **Para** no olvidar compromisos pastorales importantes

    **Criterios de aceptación:**
    ```gherkin
    Feature: Notificaciones y recordatorios
      Como pastor
      Quiero recibir notificaciones de visitas y seguimientos
      Para no olvidar compromisos

      Scenario: Recibir notificación de visita próxima
        Given que tengo visitas programadas
        When se acerca la fecha/hora
        Then recibo notificación push o email

      Scenario: Recibir recordatorio de seguimiento
        Given que marqué una visita con seguimiento requerido
        When llega la fecha de seguimiento
        Then recibo recordatorio y puedo programar nueva visita
    ```

    ### HU-PAS-10: Experiencia multidispositivo de pastor
    **Como** pastor
    **Quiero** usar la aplicación en diferentes dispositivos y en modo offline básico
    **Para** acceder desde cualquier lugar y mantener mis datos sincronizados

    **Criterios de aceptación:**
    ```gherkin
    Feature: Uso multidispositivo y offline de pastor
      Como pastor
      Quiero usar la app en móvil, tablet o desktop
      Para acceder y registrar visitas desde cualquier lugar

      Scenario: Sincronización entre dispositivos
        Given que uso la app en mi teléfono y tablet
        When registro una visita en un dispositivo
        Then veo la información actualizada en ambos

      Scenario: Modo offline básico
        Given que estoy sin conexión
        When accedo a contactos o visitas guardadas
        Then puedo ver información básica y tomar notas que se sincronizan al recuperar conexión
    ```
**Para** acceder a la gestión y supervisión desde cualquier lugar

**Criterios de aceptación:**
```gherkin
Feature: Uso multidispositivo y offline de administrador
  Como administrador
  Quiero usar la app en móvil, tablet o desktop
  Para gestionar y supervisar desde cualquier lugar

  Scenario: Sincronización entre dispositivos
    Given que uso la app en diferentes dispositivos
    When realizo una acción administrativa
    Then la información se actualiza en todos los dispositivos

  Scenario: Modo offline básico
    Given que estoy sin conexión
    When accedo a reportes o datos guardados
    Then puedo ver información básica y tomar notas que se sincronizan al recuperar conexión
```

---

### HU: Notificación de nuevos registros
**Como** administrador
**Quiero** recibir notificaciones por email cuando un usuario se registre
**Para** poder asignar roles oportunamente

**Criterios de aceptación:**
```gherkin
Feature: Notificación de nuevos registros
  Como administrador
  Quiero recibir notificaciones de nuevos usuarios
  Para gestionar su acceso

  Background:
    Given que soy administrador

  Scenario: Notificación automática por email
    Given que un usuario se registra
    Then recibo un email con los datos del usuario
    And el usuario queda pendiente de asignación de rol

  Scenario: No duplicar notificaciones
    Given que ya recibí notificación de un usuario
    When el usuario intenta registrarse de nuevo
    Then no recibo notificación duplicada
```

---

### HU: Gestión de auditoría
**Como** administrador
**Quiero** visualizar y exportar el historial de acciones realizadas por los usuarios
**Para** auditar el uso de la aplicación y detectar incidencias

**Criterios de aceptación:**
```gherkin
Feature: Auditoría de acciones de usuario
  Como administrador
  Quiero ver historial de acciones
  Para auditar el uso del sistema

  Scenario: Visualización de historial de acciones
    Given que accedo al panel de auditoría
    When consulto el historial de acciones
    Then veo la lista filtrable y exportable de acciones por usuario, fecha y módulo
```

---

### HU: Gestión de distritos e iglesias
**Como** administrador
**Quiero** crear, editar, eliminar y visualizar distritos e iglesias
**Para** mantener actualizada la estructura organizativa

**Criterios de aceptación:**
```gherkin
Feature: CRUD de distritos e iglesias
  Como administrador
  Quiero gestionar distritos e iglesias
  Para mantener la estructura organizativa

  Scenario: Crear distrito o iglesia
    Given que accedo al módulo de distritos/iglesias
    When creo un nuevo distrito o iglesia
    Then la información se guarda correctamente

  Scenario: Editar distrito o iglesia
    Given que selecciono un distrito o iglesia existente
    When modifico los datos y guardo
    Then los cambios se reflejan correctamente

  Scenario: Eliminar distrito o iglesia
    Given que selecciono un distrito o iglesia
    When presiono "Eliminar"
    Then el registro se elimina tras confirmación

  Scenario: Visualizar distritos e iglesias
    Given que accedo al módulo de distritos/iglesias
    Then veo la lista actualizada de todos los registros
```

---

### HU: Gestión de temas bíblicos (CRUD + PDF)
**Como** administrador
**Quiero** crear, editar, eliminar, visualizar y subir PDF de temas bíblicos
**Para** que los usuarios puedan consultarlos y descargarlos

**Criterios de aceptación:**
```gherkin
Feature: CRUD de temas bíblicos y PDF
  Como administrador
  Quiero gestionar temas bíblicos y sus archivos PDF
  Para que los usuarios los consulten y descarguen

  Scenario: Crear tema bíblico
    Given que accedo al módulo de temas
    When creo un nuevo tema y subo un PDF
    Then el tema y el archivo quedan disponibles para los usuarios

  Scenario: Editar tema bíblico
    Given que selecciono un tema existente
    When modifico los datos o reemplazo el PDF
    Then los cambios se reflejan correctamente

  Scenario: Eliminar tema bíblico
    Given que selecciono un tema
    When presiono "Eliminar"
    Then el tema y su PDF se eliminan tras confirmación

  Scenario: Visualizar y descargar PDF
    Given que accedo a la lista de temas
    When selecciono un tema con PDF
    Then puedo visualizarlo y descargar el archivo
```

---

### HU: Gestión de preguntas clave (CRUD)
**Como** administrador
**Quiero** crear, editar, eliminar y visualizar preguntas clave
**Para** que los usuarios puedan consultarlas en el sistema

**Criterios de aceptación:**
```gherkin
Feature: CRUD de preguntas clave
  Como administrador
  Quiero gestionar preguntas clave
  Para que los usuarios las consulten

  Scenario: Crear pregunta clave
    Given que accedo al módulo de preguntas clave
    When creo una nueva pregunta
    Then la información queda disponible para los usuarios

  Scenario: Editar pregunta clave
    Given que selecciono una pregunta existente
    When modifico el texto y guardo
    Then los cambios se reflejan correctamente

  Scenario: Eliminar pregunta clave
    Given que selecciono una pregunta
    When presiono "Eliminar"
    Then la pregunta se elimina tras confirmación

  Scenario: Visualizar preguntas clave
    Given que accedo al módulo de preguntas clave
    Then veo la lista actualizada de todas las preguntas
```

---

## 👨‍💼 HU de Pastor

### HU: Registro e inicio de sesión
**Como** pastor
**Quiero** registrarme e iniciar sesión en el sistema
**Para** acceder a las funcionalidades de gestión de visitas pastorales

**Criterios de aceptación:**
```gherkin
Feature: Registro e inicio de sesión de pastor
  Como pastor
  Quiero registrarme e iniciar sesión
  Para acceder a la gestión de visitas

  Scenario: Registro exitoso
    Given que estoy en la pantalla de registro
    When ingreso mis datos válidos
    Then el sistema crea mi cuenta y accedo al dashboard

  Scenario: Login exitoso
    Given que tengo credenciales válidas
    When ingreso email y contraseña
    Then accedo al sistema y veo mi dashboard

  Scenario: Error por email duplicado
    Given que existe un usuario con el mismo email
    When intento registrarme
    Then veo mensaje de error y no se crea la cuenta

  Scenario: Error por credenciales inválidas
    Given que ingreso credenciales incorrectas
    When intento iniciar sesión
    Then veo mensaje de error y no accedo al sistema
```

---

### HU: Gestión de contactos
**Como** pastor
**Quiero** crear, buscar, filtrar y editar contactos
**Para** organizar y programar visitas pastorales

**Criterios de aceptación:**
```gherkin
Feature: CRUD y filtrado de contactos
  Como pastor
  Quiero gestionar contactos
  Para organizar visitas

  Scenario: Crear contacto
    Given que accedo al módulo de contactos
    When ingreso los datos y guardo
    Then el contacto se crea correctamente

  Scenario: Buscar y filtrar contactos
    Given que tengo varios contactos registrados
    When uso el buscador o filtros
    Then veo solo los contactos que cumplen los criterios

  Scenario: Editar contacto
    Given que selecciono un contacto
    When modifico los datos y guardo
    Then los cambios se reflejan correctamente

  Scenario: Eliminar contacto
    Given que selecciono un contacto
    When presiono "Eliminar"
    Then el contacto se elimina tras confirmación
```

---

### HU: Gestión de visitas pastorales
**Como** pastor
**Quiero** programar, completar y registrar observaciones de visitas pastorales
**Para** mantener un historial detallado de mi ministerio

**Criterios de aceptación:**
```gherkin
Feature: Programación y registro de visitas
  Como pastor
  Quiero programar y registrar visitas
  Para mantener historial ministerial

  Scenario: Programar visita
    Given que accedo al módulo de visitas
    When selecciono contacto, fecha y hora
    Then la visita se programa correctamente

  Scenario: Completar visita
    Given que tengo una visita programada
    When la realizo y registro observaciones
    Then la visita cambia a estado "Completada" y se guarda el historial

  Scenario: Cancelar visita
    Given que tengo una visita programada
    When la cancelo con motivo
    Then la visita cambia a estado "Cancelada" y se guarda el motivo
```

---

### HU: Supervisión de líderes y familias
**Como** pastor
**Quiero** visualizar el avance y estado de los líderes y familias bajo mi supervisión
**Para** dar seguimiento y apoyo oportuno

**Criterios de aceptación:**
```gherkin
Feature: Supervisión de líderes y familias
  Como pastor
  Quiero ver el avance de líderes y familias
  Para dar seguimiento y apoyo

  Scenario: Visualizar métricas y reportes
    Given que accedo al panel de supervisión
    When consulto el estado de los líderes y familias
    Then veo métricas, reportes y detalles relevantes de cada uno
```

---

### HU: Generación de reportes
**Como** pastor
**Quiero** generar y exportar reportes de visitas, temas y preguntas clave
**Para** analizar el desempeño y compartir información con la administración

**Criterios de aceptación:**
```gherkin
Feature: Generación de reportes
  Como pastor
  Quiero generar reportes
  Para analizar desempeño y compartir información

  Scenario: Generar reporte por fechas o distrito
    Given que accedo al módulo de reportes
    When selecciono rango de fechas o distrito
    Then obtengo un archivo descargable con los datos solicitados
```

---

### HU: Gestión de perfil
**Como** pastor
**Quiero** editar y actualizar mi perfil (nombre, email, teléfono, contraseña)
**Para** mantener mi información personal actualizada y segura

**Criterios de aceptación:**
```gherkin
Feature: Edición de perfil de pastor
  Como pastor
  Quiero editar mi perfil
  Para mantener mi información actualizada

  Scenario: Editar y guardar cambios
    Given que accedo a la sección de perfil
    When modifico mis datos y guardo los cambios
    Then la información se actualiza correctamente y recibo confirmación
```

---

### HU: Notificaciones y recordatorios
**Como** pastor
**Quiero** recibir notificaciones y recordatorios de visitas programadas y seguimientos
**Para** no olvidar compromisos pastorales importantes

**Criterios de aceptación:**
```gherkin
Feature: Notificaciones y recordatorios
  Como pastor
  Quiero recibir notificaciones de visitas y seguimientos
  Para no olvidar compromisos

  Scenario: Recibir notificación de visita próxima
    Given que tengo visitas programadas
    When se acerca la fecha/hora
    Then recibo notificación push o email

  Scenario: Recibir recordatorio de seguimiento
    Given que marqué una visita con seguimiento requerido
    When llega la fecha de seguimiento
    Then recibo recordatorio y puedo programar nueva visita
```

---

### HU: Experiencia multidispositivo
**Como** pastor
**Quiero** usar la aplicación en diferentes dispositivos y en modo offline básico
**Para** acceder desde cualquier lugar y mantener mis datos sincronizados

**Criterios de aceptación:**
```gherkin
Feature: Uso multidispositivo y offline
  Como pastor
  Quiero usar la app en móvil, tablet o desktop
  Para acceder y registrar visitas desde cualquier lugar

  Scenario: Sincronización entre dispositivos
    Given que uso la app en mi teléfono y tablet
    When registro una visita en un dispositivo
    Then veo la información actualizada en ambos

  Scenario: Modo offline básico
    Given que estoy sin conexión
    When accedo a contactos o visitas guardadas
    Then puedo ver información básica y tomar notas que se sincronizan al recuperar conexión
```

---

## 🧑‍🏫 HU de Líder

### HU: Supervisión de familias
**Como** líder
**Quiero** visualizar el avance y estado de las familias asignadas
**Para** dar seguimiento y apoyo personalizado

**Criterios de aceptación:**
```gherkin
Feature: Supervisión de familias
  Como líder
  Quiero ver el avance de las familias asignadas
  Para dar seguimiento y apoyo

  Scenario: Visualizar métricas y detalles
    Given que accedo al panel de líder
    When consulto el estado de las familias
    Then veo métricas y detalles relevantes de cada familia
```

---

### HU: Gestión de familias, contactos y visitas
**Como** líder
**Quiero** gestionar las familias a mi cargo, sus contactos y visitas
**Para** dar seguimiento y reportar actividades

**Criterios de aceptación:**
```gherkin
Feature: Gestión de familias y visitas
  Como líder
  Quiero gestionar familias, contactos y visitas
  Para dar seguimiento y reportar

  Scenario: Agregar, editar o eliminar contactos y visitas
    Given que accedo al panel de líder
    When agrego, edito o elimino contactos y visitas
    Then la información se actualiza correctamente y puedo exportar reportes
```

---

### HU: Generación de reportes
**Como** líder
**Quiero** generar y exportar reportes de visitas y conceptos de las familias
**Para** analizar el desempeño y compartir información con el pastor

**Criterios de aceptación:**
```gherkin
Feature: Generación de reportes de líder
  Como líder
  Quiero generar reportes de visitas y conceptos
  Para analizar desempeño y compartir con el pastor

  Scenario: Generar reporte por familia o periodo
    Given que accedo al módulo de reportes
    When genero un reporte por familia o periodo
    Then obtengo un archivo descargable con los datos solicitados
```

---

### HU: Gestión de perfil
**Como** líder
**Quiero** editar y actualizar mi perfil (nombre, email, teléfono, contraseña)
**Para** mantener mi información personal actualizada y segura

**Criterios de aceptación:**
```gherkin
Feature: Edición de perfil de líder
  Como líder
  Quiero editar mi perfil
  Para mantener mi información actualizada

  Scenario: Editar y guardar cambios
    Given que accedo a la sección de perfil
    When modifico mis datos y guardo los cambios
    Then la información se actualiza correctamente y recibo confirmación
```

---

## 👨‍👩‍👧‍👦 HU de Familia

### HU: Gestión de conceptos (contactos y notas)
**Como** familia
**Quiero** crear, editar, eliminar y visualizar mis contactos y notas
**Para** organizar la información relevante de mis visitas y relaciones

**Criterios de aceptación:**
```gherkin
Feature: CRUD de conceptos de familia
  Como familia
  Quiero gestionar mis contactos y notas
  Para organizar información relevante

  Scenario: Crear contacto o nota
    Given que accedo al módulo de conceptos
    When creo un nuevo contacto o nota
    Then la información se guarda correctamente y es visible solo para mi familia

  Scenario: Editar o eliminar contacto o nota
    Given que selecciono un contacto o nota existente
    When edito o elimino
    Then los cambios se reflejan correctamente
```

---

### HU: Visualización y edición de visitas
**Como** familia
**Quiero** visualizar y editar mis visitas programadas y completadas
**Para** llevar un control de mi historial y próximos compromisos

**Criterios de aceptación:**
```gherkin
Feature: Visualización y edición de visitas de familia
  Como familia
  Quiero ver y editar mis visitas
  Para llevar control de mi historial y próximos compromisos

  Scenario: Ver visitas programadas y completadas
    Given que accedo al módulo de visitas
    When consulto la lista
    Then veo mis visitas programadas y el historial de completadas

  Scenario: Editar visita
    Given que selecciono una visita
    When modifico los datos y guardo
    Then la información se actualiza correctamente solo para mi familia
```

---

### HU: Gestión de perfil
**Como** familia
**Quiero** editar y actualizar mi perfil (nombre, email, teléfono, contraseña)
**Para** mantener mi información personal actualizada y segura

**Criterios de aceptación:**
```gherkin
Feature: Edición de perfil de familia
  Como familia
  Quiero editar mi perfil
  Para mantener mi información actualizada

  Scenario: Editar y guardar cambios
    Given que accedo a la sección de perfil
    When modifico mis datos y guardo los cambios
    Then la información se actualiza correctamente y recibo confirmación
```

---

# ...otros criterios generales y definiciones de terminado pueden agregarse al final si se requiere...
