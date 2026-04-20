# Ice Race

> Juego de carreras multijugador (2 jugadores) desarrollado en Unity con comunicación HTTP a servidor central Python/Flask.

---

## Descripción general del juego

**Ice Race** es un juego de carreras para **2 jugadores en tiempo real**. Cada jugador controla un pingüino y compite por llegar primero a la meta. El recorrido incluye **obstáculos** que deben esquivarse y **triggers** que al activarse reducen temporalmente la velocidad del pingüino. Ambos jugadores se ven mutuamente en pantalla y reaccionan a la posición del rival en tiempo real.

---

## Comunicación cliente-servidor

El juego utiliza un **servidor dedicado construido en Python/Flask** cuya única responsabilidad es recibir y entregar posiciones `x, y, z` de cada jugador. La comunicación se realiza exclusivamente mediante **peticiones HTTP desde Unity** usando `UnityWebRequest` dentro de Coroutines para no bloquear el hilo principal.

> **Requisito previo: Docker**
>
> **Si la imagen de Docker no está levantada, el juego no puede sincronizar el estado entre clientes y no funcionará.**
> Asegúrate de tener Docker instalado y el contenedor corriendo en `http://127.0.0.1:5000` antes de iniciar Unity.

### Endpoints utilizados

Todas las rutas siguen el patrón:

```
http://127.0.0.1:5000/server/{gameId}/{playerId}
```

| Método | Descripción |
|--------|-------------|
| `POST` | Envía `{ posX, posY, posZ }` del jugador local al servidor. Solo se envía si el jugador se movió más de `0.01` unidades respecto al último envío, evitando POST innecesarios. |
| `GET`  | Obtiene la posición del jugador remoto. Una respuesta `404` significa que el otro jugador aún no se conectó (se ignora silenciosamente). Se protege con un flag `isGetRequestPending` para evitar embotellamientos. |

### Asignación de Player ID

Cada cliente tiene un ID fijo (`0` o `1`) que se selecciona en el menú y se transfiere entre escenas mediante `PlayerTransfer` (`DontDestroyOnLoad`) o la clase estática `PlayerSession` como fallback.

### Sincronización

La posición recibida del jugador remoto se aplica con `MovePlayer()`. El movimiento del jugador local solo se habilita (`canMove = true`) cuando el servidor detecta que el rival ya envió al menos una posición diferente a la inicial, garantizando que ambos empiecen al mismo tiempo.

---

## Instrucciones para ejecutar el juego

### 1. Levantar el servidor

```bash
docker pull tu-usuario/penguin-race-server
docker run -p 5000:5000 tu-usuario/penguin-race-server
```

### 2. Abrir el proyecto en Unity

```bash
git clone (https://github.com/AngelyyP/Ice-Race.git)
# Abre la carpeta en Unity Hub (Unity 2022.3 LTS+)
# Assets → Scenes → Menu.unity
```

### 3. Ejecutar ambos clientes

Cada jugador selecciona si es "Player 1" o "Player 2" en el menú de inicio antes de entrar a la partida. Ambas instancias deben apuntar al mismo servidor. El juego espera automáticamente hasta detectar que el rival se ha conectado antes de habilitar el movimiento. Para empezar el juego debe dar click en cualquier lugar de la pantalla.

### 4. Movimientos
El movimiento del jugador se realiza mediante el teclado, utilizando un esquema simple e intuitivo:

A: Movimiento hacia la izquierda
D: Movimiento hacia la derecha

Este sistema permite al jugador maniobrar al pingüino con precisión para esquivar obstáculos y evitar zonas que reducen la velocidad durante la carrera.

---

## Requisitos técnicos

- **Unity** en la versión 6000.3.8f1
- **Docker** instalado y corriendo con la imagen del servidor Flask en el puerto `5000`
- Ambos clientes deben alcanzar la IP del servidor (misma red local o IP pública)
- Sistema operativo: Windows 10+, macOS 12+ o Linux
- GPU con soporte para DirectX 11 / Metal / Vulkan
- Mínimo 4 GB de RAM

---

## Limitaciones conocidas del sistema

- El polling introduce **latencia inherente** entre el movimiento real y el visible del rival; configurable mediante `pollRate` en el Inspector.
- Si el servidor no está activo al iniciar Unity, el juego queda bloqueado esperando conexión.
- Solo soporta exactamente **2 jugadores** con IDs `0` y `1`.
- El servidor no persiste estado entre sesiones; reiniciarlo borra la partida en curso.
- Con alta latencia de red, el suavizado puede no ser suficiente para evitar saltos visibles en la posición del rival.

---

## Capturas de pantalla
| Pantalla de inicio | Menú |
|---|---|
| ![Inicio](https://github.com/user-attachments/assets/b73886ac-2a80-42d3-a781-e2daea4c185b) | ![Menú](https://github.com/user-attachments/assets/bf36336e-d146-4ecc-ac7d-5cef478f1647) |

| Durante la carrera | Dos jugadores |
|---|---|
| ![Carrera](https://github.com/user-attachments/assets/63e87f94-fa09-496b-9b12-1d95c6d4f9a9) | ![Dos jugadores](https://github.com/user-attachments/assets/9750a299-0359-422a-b5b1-3e5e8c19c21c) |


