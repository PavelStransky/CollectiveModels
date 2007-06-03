using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Access rights
    /// </summary>
    public enum FileAccessRights {
        FILE_READ_DATA = 0x0,
        FILE_WRITE_DATA = 0x1,
        FILE_APPEND_DATA = 0x4,
        FILE_READ_EA = 0x8,
        FILE_WRITE_EA = 0x10,
        FILE_EXECUTE = 0x20,
        FILE_DELETE_CHILD = 0x40,
        FILE_READ_ATTRIBUTES = 0x80,
        FILE_WRITE_ATTRIBUTES = 0x100,
        DELETE = 0x10000,
        READ_CONTROL = 0x20000,
        WRITE_DAC = 0x40000,
        WRITE_OWNER = 0x80000,
        SYNCHRONIZE = 0x100000
    }

    /// <summary>
    /// Share mode
    /// </summary>
    public enum FileShareMode {
        FILE_SHARE_READ = 0x1,
        FILE_SHARE_WRITE = 0x2,
        FILE_SHARE_DELETE = 0x4
    }

    /// <summary>
    /// Creation disposition
    /// </summary>
    public enum FileCreationDisposition {
        CREATE_NEW = 1,
        CREATE_ALWAYS = 2,
        OPEN_EXISTING = 3,
        OPEN_ALWAYS = 4,
        TRUNCATE_EXISTING = 5
    }

    /// <summary>
    /// Flags and attributes
    /// </summary>
    public enum FileFlagsAndAttributes {
        FILE_ATTRIBUTE_READONLY = 0x1,
        FILE_ATTRIBUTE_HIDDEN = 0x2,
        FILE_ATTRIBUTE_SYSTEM = 0x4,
        FILE_ATTRIBUTE_ARCHIVE = 0x20,
        FILE_ATTRIBUTE_ENCRYPTED = 0x40,
        FILE_ATTRIBUTE_NORMAL = 0x80,
        FILE_ATTRIBUTE_TEMPORARY = 0x100,
        FILE_ATTRIBUTE_OFFLINE = 0x1000,
        FILE_ATTRIBUTE_VIRTUAL = 0x20000,

        FILE_FLAG_OPEN_NO_RECALL = 0x100000,
        FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
        FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
        FILE_FLAG_BACKUP_SEMANTICS = 0x2000000,
        FILE_FLAG_DELETE_ON_CLOSE = 0x4000000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_NO_BUFFERING = 0x20000000,
        FILE_FLAG_OVERLAPPED = 0x40000000
    }
/*
    #define CTL_CODE( DeviceType, Function, Method, Access ) (                 \
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)

    /// <summary>
    /// Control codes for drive
    /// </summary>
    public enum DriveIOControlCode {
        IOCTL_DISK_BASE = 0x7,

        IOCTL_DISK_GET_DRIVE_GEOMETRY   CTL_CODE(IOCTL_DISK_BASE, 0x0000, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_GET_PARTITION_INFO   CTL_CODE(IOCTL_DISK_BASE, 0x0001, METHOD_BUFFERED, FILE_READ_ACCESS)
#define IOCTL_DISK_SET_PARTITION_INFO   CTL_CODE(IOCTL_DISK_BASE, 0x0002, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_DISK_GET_DRIVE_LAYOUT     CTL_CODE(IOCTL_DISK_BASE, 0x0003, METHOD_BUFFERED, FILE_READ_ACCESS)
#define IOCTL_DISK_SET_DRIVE_LAYOUT     CTL_CODE(IOCTL_DISK_BASE, 0x0004, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_DISK_VERIFY               CTL_CODE(IOCTL_DISK_BASE, 0x0005, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_FORMAT_TRACKS        CTL_CODE(IOCTL_DISK_BASE, 0x0006, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_DISK_REASSIGN_BLOCKS      CTL_CODE(IOCTL_DISK_BASE, 0x0007, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_DISK_PERFORMANCE          CTL_CODE(IOCTL_DISK_BASE, 0x0008, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_IS_WRITABLE          CTL_CODE(IOCTL_DISK_BASE, 0x0009, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_LOGGING              CTL_CODE(IOCTL_DISK_BASE, 0x000a, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_FORMAT_TRACKS_EX     CTL_CODE(IOCTL_DISK_BASE, 0x000b, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_DISK_HISTOGRAM_STRUCTURE  CTL_CODE(IOCTL_DISK_BASE, 0x000c, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_HISTOGRAM_DATA       CTL_CODE(IOCTL_DISK_BASE, 0x000d, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_HISTOGRAM_RESET      CTL_CODE(IOCTL_DISK_BASE, 0x000e, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_REQUEST_STRUCTURE    CTL_CODE(IOCTL_DISK_BASE, 0x000f, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_REQUEST_DATA         CTL_CODE(IOCTL_DISK_BASE, 0x0010, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_DISK_PERFORMANCE_OFF      CTL_CODE(IOCTL_DISK_BASE, 0x0018, METHOD_BUFFERED, FILE_ANY_ACCESS)


    }*/

    /// <summary>
    /// Direct drive access
    /// </summary>
    public unsafe class DriveIO {
        [DllImport("kernel32")]
        private static extern int CreateFile(string fileName, int desiredAccess, int shareMode,
            void* securityAttributes, int creationDisposition, int flagsAndAttributes, int templateFile);


        [DllImport("kernel32")]
        private static extern int DeviceIoControl(int device, int ioControlCode, void* inBuffer,
            int inBufferSize, void* outBuffer, int outBufferSize, int* bytesReturned, void* overlapped);

    }
}
