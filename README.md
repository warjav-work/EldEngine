# Motor de Videojuegos 2D en C# | ECS + Arquitectura Limpia

## Índice
1. [Visión General](#visión-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Patrón ECS](#patrón-ecs)
4. [Estructura de Carpetas](#estructura-de-carpetas)
5. [Proyecto de Ejemplo: Elder](#proyecto-de-ejemplo-elder)
6. [Guía de Desarrollo](#guía-de-desarrollo)

---

## Visión General

Este motor es una **solución escalable, profesional y modular** para crear juegos 2D en C#. Combina:

- **Patrón ECS (Entity Component System)**: Arquitectura flexible y orientada a datos
- **Arquitectura Limpia**: Separación de responsabilidades, testeable y mantenible
- **WPF Desacoplado**: Editor visual completamente independiente del runtime
- **Prácticas Profesionales**: SOLID, DI, pattern matching, async/await

### Características Clave

✅ Runtime eficiente para juegos 2D  
✅ Editor visual WPF integrado  
✅ Sistema de scripting con C# puro  
✅ Renderizado con MonoGame/SharpDX  
✅ Gestión de escenas y niveles  
✅ Sistema de eventos desacoplado  
✅ Profiler integrado  
✅ Soporte para múltiples plataformas  

---

## Arquitectura del Sistema

### Capas Principales

```
┌─────────────────────────────────────┐
│      WPF Editor (GUI)               │ ← Interfaz visual
│   (Desacoplada del runtime)         │
├─────────────────────────────────────┤
│      Application Layer              │ ← Casos de uso
│   (EditorService, GameService)      │
├─────────────────────────────────────┤
│      Domain Layer                   │ ← Lógica de negocio
│   (ECS, Entity, Component, System)  │
├─────────────────────────────────────┤
│      Infrastructure Layer           │ ← Detalles técnicos
│   (Rendering, Input, Serialization) │
└─────────────────────────────────────┘
```

### Flujo de Comunicación

```
WPF Editor
   ↓
Application Service (Desacoplamiento via Interfaces)
   ↓
Domain (ECS Engine)
   ↓
Infrastructure (Rendering, Physics)
```

---

## Patrón ECS

### Conceptos Fundamentales

**Entity**: Identificador único que agrupa componentes
```csharp
Entity entity = world.CreateEntity();
```

**Component**: Datos puros sin lógica
```csharp
struct Position { float X, Y; }
struct Health { int Current, Max; }
```

**System**: Lógica que opera sobre entidades con componentes específicos
```csharp
class MovementSystem : ISystem
{
    public void Execute(World world)
    {
        foreach(var entity in world.GetEntitiesWith<Position, Velocity>())
        {
            entity.Get<Position>().X += entity.Get<Velocity>().X;
        }
    }
}
```

### Ventajas del ECS

| Aspecto | ECS | OOP Tradicional |
|--------|-----|-----------------|
| **Flexibilidad** | Alta (composición) | Media (herencia) |
| **Performance** | Excelente (caché) | Normal |
| **Reutilización** | Máxima | Variable |
| **Testing** | Fácil | Complejo |

---

## Estructura de Carpetas

```
ElEngine.sln
├── ElEngine.Core/                 # Núcleo del motor
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── Entity.cs
│   │   │   ├── IComponent.cs
│   │   │   └── ComponentStorage.cs
│   │   ├── Systems/
│   │   │   ├── ISystem.cs
│   │   │   ├── SystemManager.cs
│   │   │   └── SystemOrder.cs
│   │   ├── World/
│   │   │   ├── World.cs
│   │   │   ├── IWorldListener.cs
│   │   │   └── Events/
│   │   └── Values/                # Structs de datos
│   │       ├── Transform.cs
│   │       ├── Color.cs
│   │       └── Vector2.cs
│   ├── Application/
│   │   ├── Interfaces/
│   │   │   ├── IGameService.cs
│   │   │   ├── IRenderService.cs
│   │   │   └── IInputService.cs
│   │   └── Services/
│   │       ├── GameService.cs
│   │       └── SceneService.cs
│   └── Infrastructure/
│       ├── Rendering/
│       │   ├── SharpDXRenderer.cs
│       │   └── RenderContext.cs
│       ├── Input/
│       │   └── InputManager.cs
│       └── Serialization/
│           ├── SceneSerializer.cs
│           └── EntityBlueprint.cs
│
├── ElEngine.Editor/               # Editor WPF
│   ├── Views/
│   │   ├── MainWindow.xaml
│   │   ├── ViewModels/
│   │   └── Controls/
│   ├── Services/
│   │   ├── EditorService.cs
│   │   └── ProjectService.cs
│   └── App.xaml
│
├── Elder.Game/                  # Proyecto de ejemplo
│   ├── Components/
│   │   ├── PlayerController.cs
│   │   ├── NpcBehavior.cs
│   │   └── CombatStats.cs
│   ├── Systems/
│   │   ├── PlayerInputSystem.cs
│   │   ├── AiSystem.cs
│   │   └── CombatSystem.cs
│   ├── Scenes/
│   │   ├── MainScene.cs
│   │   └── CombatScene.cs
│   └── Assets/
│       ├── Sprites/
│       ├── Levels/
│       └── Scripts/
│
└── ElEngine.Tests/                # Tests unitarios
    ├── ECS/
    ├── Systems/
    └── Integration/
```

---

## Proyecto de Ejemplo: Elder

### "Elder: Chronicles of the Silver Grove"

**Género**: Aventura 2D narrativa con combates ligeros  
**Mecánicas**:
- Exploración del bosque élfico
- Diálogo con NPCs
- Sistema de combate en tiempo real
- Inventario y equipamiento

### Escenas Principales

1. **MainScene**: Bosque élfico, exploración libre
2. **CombatScene**: Sistema de combate arena
3. **VillageScene**: Aldea élfica con NPCs
4. **TownScene**: Ruinas antiguas

### Componentes Específicos

```csharp
// Jugador
- Transform, Sprite, Collider
- PlayerController (custom)
- CombatStats (HP, Mana, Ataque)
- Inventory

// NPCs
- Transform, Sprite, Collider
- NpcBehavior (conversación, IA)
- DialogueTree
- SimpleAI

// Enemigos
- Transform, Sprite, Collider
- EnemyAI
- CombatStats
- LootDrop
```

### Flujo de Juego

```
Start Game
  ↓
Load MainScene
  ↓
Player explores → Finds NPC → Dialogue
  ↓
Encounters Enemy → Enter Combat
  ↓
Combat Resolution → Loot/Continue
  ↓
Return to MainScene
```

---

## Guía de Desarrollo

### Fase 1: MVP (Semana 1-2)

1. Implementar ECS core
2. Renderizador básico (SharpDX)
3. Sistema de input
4. Una escena simple con enemigos

### Fase 2: Editor (Semana 3-4)

1. WPF UI básica
2. Inspector de entidades
3. Scene viewer
4. Serialización entity blueprints

### Fase 3: Gameplay (Semana 5-6)

1. Sistema de combate
2. Diálogos y NPCs
3. Inventario
4. Sonido y efectos

### Fase 4: Pulido (Semana 7-8)

1. Performance profiling
2. Balance de juego
3. Efectos visuales
4. UI mejorada

---

## Principios de Diseño

### SOLID

- **S**ingle Responsibility: Cada system tiene una responsabilidad
- **O**pen/Closed: Fácil agregar nuevos sistemas sin modificar existentes
- **L**iskov Substitution: Sistemas intercambiables
- **I**nterface Segregation: Interfaces específicas (ISystem, IComponent)
- **D**ependency Inversion: Inyección de dependencias

### Patrones Utilizados

| Patrón | Uso |
|--------|-----|
| **Service Locator** | GameService, RenderService |
| **Factory** | EntityFactory, ComponentFactory |
| **Observer** | EventSystem, WorldListeners |
| **Strategy** | Diferentes sistemas de render/input |
| **Composite** | Transform hierarchy |

### Buenas Prácticas

✅ Separación clara de concerns  
✅ Código agnóstico del motor gráfico  
✅ Testeable sin dependencias externas  
✅ Mensajes de error descriptivos  
✅ Documentación por código (XML docs)  
✅ Logging integral  
✅ Hot-reload preparado  

---

## Próximos Pasos

1. **Rendering Avanzado**: Tiles, sprites animados, parallax
2. **Multithreading**: Job system para sistemas paralelos
3. **Plugin System**: Cargar sistemas en runtime
4. **Physics**: Detección de colisiones avanzada
5. **Networking**: Multiplayer básico
6. **Tools**: Asset pipeline, batcher, profiler visual

---

## Conclusión

Este motor proporciona una **base sólida, escalable y profesional** para desarrollar juegos 2D complejos. La combinación de ECS + Arquitectura Limpia garantiza mantenibilidad y extensibilidad a largo plazo.

El proyecto Elder demuestra cómo aplicar estos conceptos en un juego real, sirviendo como guía para proyectos futuros.