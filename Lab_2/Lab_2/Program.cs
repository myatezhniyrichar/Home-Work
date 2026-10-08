using System;
using System.Collections.Generic;

// ===================== Интерфейсы =====================

public interface Controllable
{
    void turnOn();
    void turnOff();
    void updateStatus();
}

public interface AutomationAction
{
    void execute();
}

// ===================== Перечисления =====================

public enum RoomType
{
    Kitchen,
    LivingRoom,
    Bedroom
}

// ===================== Комната =====================

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RoomType Type { get; set; }

    public Room(int id, string name, RoomType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public override string ToString() => $"Комната[{Id}] {Name} ({Type})";
}

// ===================== Базовое устройство =====================

public abstract class SmartDevice : Controllable
{
    private string _status = "выкл";

    public int Id { get; protected set; }
    public string Name { get; protected set; }
    public string Manufacturer { get; protected set; }
    public Room Room { get; set; }

    public string Status => _status;

    protected SmartDevice(int id, string name, string manufacturer, Room room)
    {
        Id = id;
        Name = name;
        Manufacturer = manufacturer;
        Room = room;
    }

    public void setState(string newState)
    {
        if (newState != "вкл" && newState != "выкл" && newState != "недоступен")
            throw new ArgumentException($"Недопустимое состояние: {newState}");
        _status = newState;
    }

    public abstract string getDeviceType();
    public abstract void executeCommand(string command);

    public virtual void turnOn() => setState("вкл");
    public virtual void turnOff() => setState("выкл");

    public virtual void updateStatus()
        => Console.WriteLine($"{Name}: {Status}");

    public override string ToString()
        => $"{getDeviceType()} [{Id}] {Name} ({Manufacturer}) — {Status}";
}

// ===================== Умная лампа =====================

public class SmartLight : SmartDevice
{
    private int _brightness;

    public int Brightness
    {
        get => _brightness;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Яркость должна быть в диапазоне 0–100");
            _brightness = value;
        }
    }

    public SmartLight(int id, string name, string manufacturer, Room room, int brightness = 50)
        : base(id, name, manufacturer, room)
    {
        Brightness = brightness;
    }

    public override string getDeviceType() => "SmartLight";

    public override void executeCommand(string command)
    {
        switch (command.ToLower())
        {
            case "on":
            case "вкл":
                turnOn();
                Console.WriteLine($"{Name} включён");
                break;
            case "off":
            case "выкл":
                turnOff();
                Console.WriteLine($"{Name} выключен");
                break;
            case "bright_up":
                Brightness = Math.Min(100, Brightness + 10);
                Console.WriteLine($"{Name}: яркость → {Brightness}");
                break;
            case "bright_down":
                Brightness = Math.Max(0, Brightness - 10);
                Console.WriteLine($"{Name}: яркость → {Brightness}");
                break;
            default:
                Console.WriteLine($"{Name}: неизвестная команда '{command}'");
                break;
        }
    }

    public override void updateStatus()
        => Console.WriteLine($"{Name}: {Status}, яркость = {Brightness}");
}

// ===================== Умный термостат =====================

public class SmartThermostat : SmartDevice
{
    public double CurrentTemperature { get; set; }
    public double TargetTemperature { get; set; }

    public SmartThermostat(int id, string name, string manufacturer, Room room,
                           double currentTemp, double targetTemp)
        : base(id, name, manufacturer, room)
    {
        CurrentTemperature = currentTemp;
        TargetTemperature = targetTemp;
    }

    public override string getDeviceType() => "SmartThermostat";

    public override void executeCommand(string command)
    {
        string cmd = command.ToLower();
        if (cmd.StartsWith("set ") && double.TryParse(cmd.Substring(4), out double t))
        {
            TargetTemperature = t;
            Console.WriteLine($"{Name}: целевая температура → {TargetTemperature}°C");
        }
        else if (cmd == "on" || cmd == "вкл") { turnOn(); Console.WriteLine($"{Name} включён"); }
        else if (cmd == "off" || cmd == "выкл") { turnOff(); Console.WriteLine($"{Name} выключен"); }
        else Console.WriteLine($"{Name}: неизвестная команда '{command}'");
    }

    public override void updateStatus()
        => Console.WriteLine($"{Name}: {Status}, текущая {CurrentTemperature}°C, " +
                             $"целевая {TargetTemperature}°C");
}

// ===================== Умный замок =====================

public class SmartLock : SmartDevice
{
    private string _lockStatus = "закрыт";
    public string LockStatus => _lockStatus;

    public SmartLock(int id, string name, string manufacturer, Room room)
        : base(id, name, manufacturer, room) { }

    public void setLockStatus(string newStatus)
    {
        if (newStatus != "открыт" && newStatus != "закрыт")
            throw new ArgumentException("Статус замка: 'открыт' или 'закрыт'");
        _lockStatus = newStatus;
    }

    public override string getDeviceType() => "SmartLock";

    public override void executeCommand(string command)
    {
        switch (command.ToLower())
        {
            case "open":
            case "открыть":
                setLockStatus("открыт");
                Console.WriteLine($"{Name}: открыт");
                break;
            case "close":
            case "закрыть":
                setLockStatus("закрыт");
                Console.WriteLine($"{Name}: закрыт");
                break;
            case "on": case "вкл": turnOn(); break;
            case "off": case "выкл": turnOff(); break;
            default:
                Console.WriteLine($"{Name}: неизвестная команда '{command}'");
                break;
        }
    }

    public override void updateStatus()
        => Console.WriteLine($"{Name}: {Status}, замок {LockStatus}");
}

// ===================== Действие автоматизации =====================

public class DeviceCommandAction : AutomationAction
{
    public SmartDevice Device { get; }
    public string Command { get; }
    public string Description { get; }

    public DeviceCommandAction(SmartDevice device, string command, string description = null)
    {
        Device = device;
        Command = command;
        Description = description ?? $"{device.Name}: команда '{command}'";
    }

    public void execute()
    {
        Console.WriteLine($"  → {Description}");
        Device.executeCommand(Command);
    }
}

// ===================== Сценарий автоматизации =====================

public class AutomationScenario
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TriggerCondition { get; set; }
    public List<AutomationAction> Actions { get; } = new List<AutomationAction>();

    public AutomationScenario(int id, string name, string triggerCondition)
    {
        Id = id;
        Name = name;
        TriggerCondition = triggerCondition;
    }

    public void AddAction(AutomationAction action) => Actions.Add(action);

    public void Run()
    {
        Console.WriteLine($"=== Сценарий '{Name}' (условие: {TriggerCondition}) ===");
        foreach (var action in Actions)
            action.execute();
        Console.WriteLine();
    }
}

// ===================== Демонстрация =====================

class Program
{
    static void Main()
    {
        var kitchen = new Room(1, "Кухня", RoomType.Kitchen);
        var living = new Room(2, "Гостиная", RoomType.LivingRoom);
        var bedroom = new Room(3, "Спальня", RoomType.Bedroom);

        var devices = new List<SmartDevice>
        {
            new SmartLight(101, "Люстра",         "Xiaomi",  living,  80),
            new SmartLight(102, "Ночник",         "Philips", bedroom, 30),
            new SmartThermostat(201, "Термостат", "Nest",    living,  22.5, 23.0),
            new SmartLock(301, "Входной замок",   "Yale",    living)
        };

        Console.WriteLine("Устройства дома:");
        foreach (var d in devices) Console.WriteLine("  " + d);
        Console.WriteLine();

        Console.WriteLine("Полиморфное управление через executeCommand():");
        devices[0].executeCommand("on");
        devices[0].executeCommand("bright_up");
        devices[1].executeCommand("on");
        devices[2].executeCommand("set 24");
        devices[3].executeCommand("open");
        Console.WriteLine();

        try
        {
            ((SmartLight)devices[0]).Brightness = 150;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"[Проверка] {ex.Message}");
        }
        Console.WriteLine();

        var evening = new AutomationScenario(1, "Вечерний режим", "22:00 / все уходят спать");
        evening.AddAction(new DeviceCommandAction(devices[0], "off",
            "Выключить люстру в гостиной"));
        evening.AddAction(new DeviceCommandAction(devices[2], "set 20",
            "Установить температуру 20°C"));
        evening.AddAction(new DeviceCommandAction(devices[3], "close",
            "Закрыть входную дверь"));
        evening.AddAction(new DeviceCommandAction(devices[1], "on",
            "Включить ночник в спальне"));
        evening.AddAction(new DeviceCommandAction(devices[1], "bright_down",
            "Приглушить ночник"));

        evening.Run();

        Console.WriteLine("Текущее состояние всех устройств:");
        foreach (var d in devices) d.updateStatus();
    }
}