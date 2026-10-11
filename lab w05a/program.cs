/****************/
// Mã sinh viên: 20227234
// Họ tên: Bùi Phạm Quang Huy
/****************/

using System;
using System.Collections.Generic;

namespace LabDeviceManagement
{
    // Enum trạng thái thiết bị
    public enum DeviceStatus
    {
        Active,
        UnderMaintenance,
        Retired
    }

    // Interface định nghĩa năng lực kết nối mạng
    public interface INetworkable
    {
        string IpAddress { get; }
        void Connect(string ipAddress);
        void Disconnect();
        bool IsConnected { get; }
    }

    // Lớp trừu tượng Device
    public abstract class Device
    {
        public string DeviceId { get; }
        public string Name { get; set; }
        public int UsageYear { get; set; }
        public decimal PurchasePrice { get; set; }
        public DeviceStatus Status { get; set; }

        protected Device(string deviceId, string name, int usageYear, decimal purchasePrice, DeviceStatus status)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Mã thiết bị không được rỗng.", nameof(deviceId));
            if (purchasePrice <= 0)
                throw new ArgumentException("Giá mua phải lớn hơn 0.", nameof(purchasePrice));
            if (usageYear > DateTime.Now.Year)
                throw new ArgumentException("Năm đưa vào sử dụng không được lớn hơn năm hiện tại.", nameof(usageYear));

            DeviceId = deviceId;
            Name = name;
            UsageYear = usageYear;
            PurchasePrice = purchasePrice;
            Status = status;
        }

        // Phương thức trừu tượng tính chi phí bảo trì dự kiến trong một năm
        public abstract decimal CalculateAnnualMaintenanceCost();

        // Kiểm tra thiết bị đã sử dụng trên 5 năm hay chưa
        public bool IsOlderThan5Years()
        {
            return (DateTime.Now.Year - UsageYear) > 5;
        }

        public override string ToString()
        {
            return $"[ID: {DeviceId}] Tên: {Name}, Năm sử dụng: {UsageYear}, Giá: {PurchasePrice:N0} VNĐ, Trạng thái: {Status}";
        }
    }

    // Lớp Computer kế thừa Device và thực thi INetworkable
    public class Computer : Device, INetworkable
    {
        public int RamCapacity { get; set; } // GB
        public string CpuType { get; set; }
        public bool HasDiscreteGpu { get; set; }

        // Thuộc tính của INetworkable
        public string IpAddress { get; private set; } = string.Empty;
        public bool IsConnected { get; private set; } = false;

        public Computer(string deviceId, string name, int usageYear, decimal purchasePrice, DeviceStatus status,
                        int ramCapacity, string cpuType, bool hasDiscreteGpu)
            : base(deviceId, name, usageYear, purchasePrice, status)
        {
            RamCapacity = ramCapacity;
            CpuType = cpuType;
            HasDiscreteGpu = hasDiscreteGpu;
        }

        public override decimal CalculateAnnualMaintenanceCost()
        {
            decimal cost = PurchasePrice * 0.05m;
            if (HasDiscreteGpu)
            {
                cost += PurchasePrice * 0.02m;
            }
            if (IsOlderThan5Years())
            {
                cost += PurchasePrice * 0.01m;
            }
            return cost;
        }

        // Triển khai INetworkable
        public void Connect(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("Địa chỉ IP không được rỗng.");
            if (IsConnected)
                throw new InvalidOperationException("Thiết bị đang kết nối, không thể kết nối lại.");

            IpAddress = ipAddress;
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
            IpAddress = string.Empty;
        }
    }

    // Lớp Printer cơ sở
    public class Printer : Device
    {
        public string PrinterType { get; set; } // Laser hoặc Phun
        public int PagesPrinted { get; set; }
        public bool IsColorPrinter { get; set; }

        public Printer(string deviceId, string name, int usageYear, decimal purchasePrice, DeviceStatus status,
                       string printerType, int pagesPrinted, bool isColorPrinter)
            : base(deviceId, name, usageYear, purchasePrice, status)
        {
            PrinterType = printerType;
            PagesPrinted = pagesPrinted;
            IsColorPrinter = isColorPrinter;
        }

        public override decimal CalculateAnnualMaintenanceCost()
        {
            decimal cost = PurchasePrice * 0.04m;
            if (PagesPrinted > 100000)
            {
                cost += 500000m;
            }
            if (IsColorPrinter)
            {
                cost += 300000m;
            }
            return cost;
        }
    }

    // Lớp NetworkPrinter kế thừa Printer và thực thi INetworkable
    public class NetworkPrinter : Printer, INetworkable
    {
        public string IpAddress { get; private set; } = string.Empty;
        public bool IsConnected { get; private set; } = false;

        public NetworkPrinter(string deviceId, string name, int usageYear, decimal purchasePrice, DeviceStatus status,
                              string printerType, int pagesPrinted, bool isColorPrinter)
            : base(deviceId, name, usageYear, purchasePrice, status, printerType, pagesPrinted, isColorPrinter)
        {
        }

        public void Connect(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("Địa chỉ IP không được rỗng.");
            if (IsConnected)
                throw new InvalidOperationException("Thiết bị đang kết nối, không thể kết nối lại.");

            IpAddress = ipAddress;
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
            IpAddress = string.Empty;
        }
    }

    // Lớp Projector
    public class Projector : Device
    {
        public int Lumen { get; set; }
        public int LampHoursUsed { get; set; }

        public Projector(string deviceId, string name, int usageYear, decimal purchasePrice, DeviceStatus status,
                         int lumen, int lampHoursUsed)
            : base(deviceId, name, usageYear, purchasePrice, status)
        {
            Lumen = lumen;
            LampHoursUsed = lampHoursUsed;
        }

        public override decimal CalculateAnnualMaintenanceCost()
        {
            decimal cost = PurchasePrice * 0.03m;
            if (LampHoursUsed > 3000)
            {
                cost += 1500000m;
            }
            return cost;
        }
    }

    // Lớp LabRoom quản lý thiết bị bằng kết tập
    public class LabRoom
    {
        public string RoomId { get; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        private readonly List<Device> _devices;

        public LabRoom(string roomId, string roomName, int capacity)
        {
            RoomId = roomId;
            RoomName = roomName;
            Capacity = capacity;
            _devices = new List<Device>();
        }

        public void AddDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device), "Không chấp nhận đối tượng thiết bị null.");
            if (FindDevice(device.DeviceId) != null)
                throw new InvalidOperationException($"Thiết bị với mã '{device.DeviceId}' đã tồn tại trong phòng.");

            _devices.Add(device);
        }

        public bool RemoveDevice(string deviceId)
        {
            var device = FindDevice(deviceId);
            if (device != null)
            {
                _devices.Remove(device);
                return true;
            }
            return false;
        }

        public Device? FindDevice(string deviceId)
        {
            return _devices.Find(d => d.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase));
        }

        // Tính tổng chi phí bảo trì sử dụng tính đa hình
        public decimal CalculateAnnualMaintenanceCost()
        {
            decimal totalCost = 0;
            foreach (var device in _devices)
            {
                totalCost += device.CalculateAnnualMaintenanceCost();
            }
            return totalCost;
        }

        // Liệt kê thiết bị cần bảo trì
        public List<Device> GetDevicesRequiringMaintenance()
        {
            var result = new List<Device>();
            foreach (var device in _devices)
            {
                if (device.Status == DeviceStatus.UnderMaintenance || device.IsOlderThan5Years())
                {
                    result.Add(device);
                }
            }
            return result;
        }

        public IReadOnlyList<Device> Devices => _devices.AsReadOnly();
    }

    // Chương trình chính kiểm thử tối thiểu
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // 1. Khởi tạo đối tượng theo yêu cầu kiểm thử
            var pc1 = new Computer("PC01", "Máy tính Dell Core i5", 2023, 15000000m, DeviceStatus.Active, 16, "Intel i5", false);
            var pc2 = new Computer("PC02", "Máy tính Gaming Asus", 2019, 25000000m, DeviceStatus.Active, 32, "Intel i7", true);

            var printer1 = new Printer("PR01", "Máy in HP Laser", 2024, 4000000m, DeviceStatus.Active, "Laser", 50000, false);
            var printer2 = new NetworkPrinter("PR02", "Máy in màu Canon mạng", 2020, 7000000m, DeviceStatus.UnderMaintenance, "Phun", 120000, true);

            var projector1 = new Projector("PJ01", "Máy chiếu Sony", 2018, 12000000m, DeviceStatus.Active, 3500, 3500);

            // 2. Tạo 2 phòng thực hành
            var lab1 = new LabRoom("LAB1", "Phòng Thực hành 1", 30);
            var lab2 = new LabRoom("LAB2", "Phòng Thực hành 2", 25);

            // Thêm thiết bị vào phòng
            lab1.AddDevice(pc1);
            lab1.AddDevice(pc2);
            lab1.AddDevice(printer1);

            lab2.AddDevice(printer2);
            lab2.AddDevice(projector1);

            // Kiểm tra thêm trùng mã (Yêu cầu 2)
            try
            {
                lab1.AddDevice(new Computer("PC01", "Máy tính trùng mã", 2025, 10000000m, DeviceStatus.Active, 8, "i3", false));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Test Trùng Mã Thành Công]: {ex.Message}");
            }

            // In danh sách thiết bị từng phòng (Yêu cầu 3)
            Console.WriteLine($"\n--- Danh sách thiết bị {lab1.RoomName} ---");
            foreach (var d in lab1.Devices) Console.WriteLine(d);

            // Tính tổng chi phí bảo trì dự kiến (Yêu cầu 4)
            Console.WriteLine($"\nTổng chi phí bảo trì {lab1.RoomName}: {lab1.CalculateAnnualMaintenanceCost():N0} VNĐ");
            Console.WriteLine($"Tổng chi phí bảo trì {lab2.RoomName}: {lab2.CalculateAnnualMaintenanceCost():N0} VNĐ");

            // Liệt kê thiết bị cần bảo trì (Yêu cầu 5)
            Console.WriteLine($"\n--- Thiết bị cần bảo trì ở {lab2.RoomName} ---");
            foreach (var d in lab2.GetDevicesRequiringMaintenance())
            {
                Console.WriteLine($"- {d.Name} (Trạng thái: {d.Status}, Năm SD: {d.UsageYear})");
            }

            // Kết nối mạng cho đối tượng INetworkable (Yêu cầu 6 & 7)
            Console.WriteLine("\n--- Kiểm tra kết nối mạng (INetworkable) ---");
            List<INetworkable> networkDevices = new List<INetworkable> { pc1, pc2, printer2 };
            foreach (var netDev in networkDevices)
            {
                netDev.Connect("192.168.1.100");
                Console.WriteLine($"Đã kết nối IP: {netDev.IpAddress} | Trạng thái IsConnected: {netDev.IsConnected}");
            }

            Console.WriteLine("\nHoàn thành kiểm thử hệ thống!");
        }
    }
}
