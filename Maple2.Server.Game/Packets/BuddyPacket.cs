using Maple2.Model.Error;
using Maple2.Model.Game;
using Maple2.PacketLib.Tools;
using Maple2.Server.Core.Constants;
using Maple2.Server.Core.Packets;
using Maple2.Tools.Extensions;

namespace Maple2.Server.Game.Packets;

public static class BuddyPacket {
    private enum Command : byte {
        Load = 1,
        Invite = 2,
        Accept = 3,
        Decline = 4,
        Block = 5,
        Unblock = 6,
        Remove = 7,
        UpdateInfo = 8,
        Append = 9,
        UpdateBlock = 10,
        NotifyAccept = 11,
        NotifyBlock = 12,
        NotifyRemove = 13,
        NotifyOnline = 14,
        StartList = 15,
        Cancel = 17,
        Forbidden = 18,
        EndList = 19,
        Unknown = 20,
    }

    private static ByteWriter Start(Command command) {
        var pWriter = Packet.Of(SendOp.Buddy);
        pWriter.Write<Command>(command);
        return pWriter;
    }

    public static ByteWriter Load(ICollection<Buddy> buddies, ICollection<Buddy> blocked) {
        var pWriter = Start(Command.Load);
        pWriter.WriteInt(buddies.Count + blocked.Count);
        foreach (Buddy buddy in buddies) {
            pWriter.WriteClass<Buddy>(buddy);
        }
        foreach (Buddy buddy in blocked) {
            pWriter.WriteClass<Buddy>(buddy);
        }

        return pWriter;
    }

    public static ByteWriter Invite(string name = "", string message = "", BuddyError error = BuddyError.ok) {
        var pWriter = Start(Command.Invite);
        pWriter.Write<BuddyError>(error);
        pWriter.WriteUnicodeString(name);
        pWriter.WriteUnicodeString(message);

        return pWriter;
    }

    public static ByteWriter Accept(Buddy buddy) {
        var pWriter = Start(Command.Accept);
        pWriter.Write<BuddyError>(BuddyError.ok);
        pWriter.WriteLong(buddy.Id);
        pWriter.WriteLong(buddy.Info.CharacterId);
        pWriter.WriteLong(buddy.Info.AccountId);
        pWriter.WriteUnicodeString(buddy.Info.Name);

        return pWriter;
    }

    public static ByteWriter Decline(Buddy buddy) {
        var pWriter = Start(Command.Decline);
        pWriter.Write<BuddyError>(BuddyError.ok);
        pWriter.WriteLong(buddy.Id);

        return pWriter;
    }

    public static ByteWriter Block(long entryId = 0, string name = "", string message = "", BuddyError error = BuddyError.ok) {
        var pWriter = Start(Command.Block);
        pWriter.Write<BuddyError>(error);
        pWriter.WriteLong(entryId);
        pWriter.WriteUnicodeString(name);
        pWriter.WriteUnicodeString(message);

        return pWriter;
    }

    public static ByteWriter Unblock(Buddy buddy) {
        var pWriter = Start(Command.Unblock);
        pWriter.Write<BuddyError>(BuddyError.ok);
        pWriter.WriteLong(buddy.Id);

        return pWriter;
    }

    public static ByteWriter Remove(Buddy buddy) {
        var pWriter = Start(Command.Remove);
        pWriter.Write<BuddyError>(BuddyError.ok);
        pWriter.WriteLong(buddy.Id);
        pWriter.WriteLong(buddy.Info.CharacterId);
        pWriter.WriteLong(buddy.Info.AccountId);
        pWriter.WriteUnicodeString(buddy.Info.Name);

        return pWriter;
    }

    public static ByteWriter UpdateInfo(Buddy buddy) {
        var pWriter = Start(Command.UpdateInfo);
        pWriter.WriteClass<Buddy>(buddy);

        return pWriter;
    }

    public static ByteWriter Append(Buddy buddy) {
        var pWriter = Start(Command.Append);
        pWriter.WriteClass<Buddy>(buddy);

        return pWriter;
    }

    public static ByteWriter UpdateBlock(long entryId = 0, string name = "", string message = "", BuddyError error = BuddyError.ok) {
        var pWriter = Start(Command.UpdateBlock);
        pWriter.Write<BuddyError>(error);
        pWriter.WriteLong(entryId);
        pWriter.WriteUnicodeString(name);
        pWriter.WriteUnicodeString(message);

        return pWriter;
    }

    public static ByteWriter NotifyAccept(Buddy buddy) {
        var pWriter = Start(Command.NotifyAccept);
        pWriter.WriteLong(buddy.Id);

        return pWriter;
    }

    public static ByteWriter NotifyBlock(string name, BuddyError error = BuddyError.ok) {
        var pWriter = Start(Command.NotifyBlock);
        pWriter.Write<BuddyError>(error);
        pWriter.WriteUnicodeString(name);

        return pWriter;
    }

    public static ByteWriter NotifyRemove(Buddy buddy, string action) {
        var pWriter = Start(Command.NotifyRemove);
        pWriter.WriteInt();
        pWriter.WriteUnicodeString(buddy.Info.Name);
        pWriter.WriteUnicodeString(action);
        pWriter.WriteLong(buddy.Id);

        return pWriter;
    }

    public static ByteWriter NotifyOnline(Buddy buddy) {
        var pWriter = Start(Command.NotifyOnline);
        pWriter.WriteBool(!buddy.Info.Online); // true == offline
        pWriter.WriteLong(buddy.Id);
        pWriter.WriteUnicodeString(buddy.Info.Name);

        return pWriter;
    }

    public static ByteWriter StartList() {
        var pWriter = Start(Command.StartList);

        return pWriter;
    }

    public static ByteWriter Cancel(Buddy buddy) {
        var pWriter = Start(Command.Cancel);
        pWriter.Write<BuddyError>(BuddyError.ok);
        pWriter.WriteLong(buddy.Id);

        return pWriter;
    }

    // s_ban_check_err_any: Contains a forbidden word.
    public static ByteWriter Forbidden() {
        var pWriter = Start(Command.Forbidden);

        return pWriter;
    }

    public static ByteWriter EndList(int count) {
        var pWriter = Start(Command.EndList);
        pWriter.WriteInt(count);

        return pWriter;
    }

    public static ByteWriter Unknown() {
        var pWriter = Start(Command.Unknown);

        return pWriter;
    }
}
