using System.Web;
using Maple2.Model.Enum;
using Maple2.Model.Game;
using Maple2.PacketLib.Tools;
using Maple2.Server.Core.Constants;
using Maple2.Server.Core.Packets;

namespace Maple2.Server.Game.Packets;

public static class ChatPacket {
    private static ByteWriter StartChat(long accountId, long characterId, string name, string message, ChatType type, bool htmlOrCodeFlag = false) {
        var pWriter = Packet.Of(SendOp.UserChat);
        pWriter.WriteLong(accountId);
        pWriter.WriteLong(characterId);
        pWriter.WriteUnicodeString(name);
        pWriter.WriteBool(htmlOrCodeFlag);
        pWriter.WriteUnicodeString(message);
        pWriter.Write<ChatType>(type);
        pWriter.WriteBool(false);
        pWriter.WriteInt();
        return pWriter;
    }

    private static ByteWriter FinishChat(ByteWriter pWriter) {
        pWriter.WriteBool(false);
        return pWriter;
    }

    public static ByteWriter Message(Player player, ChatType type, string message) {
        var pWriter = StartChat(player.Account.Id, player.Character.Id, player.Character.Name, message, type);

        switch (type) {
            case ChatType.WhisperFrom:
                pWriter.WriteUnicodeString();
                break;
            case ChatType.Super:
                pWriter.WriteInt();
                break;
            case ChatType.Club:
                pWriter.WriteLong();
                break;
        }

        return FinishChat(pWriter);
    }

    public static ByteWriter Message(long accountId, long characterId, string characterName, ChatType type, string message, int superChatId = 0, long clubId = 0) {
        var pWriter = StartChat(accountId, characterId, characterName, message, type);

        switch (type) {
            case ChatType.WhisperFrom:
                pWriter.WriteUnicodeString();
                break;
            case ChatType.Super:
                pWriter.WriteInt(superChatId);
                break;
            case ChatType.Club:
                pWriter.WriteLong(clubId);
                break;
        }

        return FinishChat(pWriter);
    }

    public static ByteWriter Whisper(long accountId, long characterId, string name, string message, string? unknown = null) {
        ChatType type = unknown == null ? ChatType.WhisperTo : ChatType.WhisperFrom;
        var pWriter = StartChat(accountId, characterId, name, message, type);

        if (type == ChatType.WhisperFrom) {
            pWriter.WriteUnicodeString(unknown ?? string.Empty);
        }

        return FinishChat(pWriter);
    }

    public static ByteWriter WhisperReject(string name) {
        var pWriter = StartChat(0, 0, name, ".", ChatType.WhisperReject);
        return FinishChat(pWriter);
    }

    public static ByteWriter System(string type, string message) {
        var pWriter = StartChat(0, 0, type, message, ChatType.System);
        return FinishChat(pWriter);
    }

    public static ByteWriter Alert(StringCode code) {
        // In this variant, the message payload is the StringCode written directly; keep sequence consistent
        var pWriter = Packet.Of(SendOp.UserChat);
        pWriter.WriteLong();
        pWriter.WriteLong();
        pWriter.WriteUnicodeString();
        pWriter.WriteBool(true);
        pWriter.Write<StringCode>(code);
        pWriter.Write<ChatType>(ChatType.NoticeAlert);
        pWriter.WriteBool(false);
        pWriter.WriteInt();
        return FinishChat(pWriter);
    }

    public static ByteWriter Alert(string message, bool htmlEncoded = false) {
        var pWriter = StartChat(0, 0, string.Empty, htmlEncoded ? message : HttpUtility.HtmlEncode(message), ChatType.NoticeAlert);
        return FinishChat(pWriter);
    }
}
