using System;
using Catel.Data;

namespace DesktopTelegramBot.Models
{
	class ChatMessage : ModelBase
	{
		public int Id { get; set; }
		public int UserId { get; private set; }
		public Chat Chat { get; private set; }
		public DateTime DateTime { get; private set; }
		public string Message { get; private set; }
		public ChatMessageType Type { get; private set; }

		public bool IsIncoming => Type == ChatMessageType.Incoming;
		public bool IsOutgoing => Type == ChatMessageType.Outgoing;

		private ChatMessage()
		{
		}

		public static ChatMessage Create( int id, int userId, Chat chat, DateTime dateTime, ChatMessageType type, string message )
		{
			return new ChatMessage
			{
				Id = id,
				UserId = userId,
				Chat = chat,
				DateTime = dateTime,
				Type = type,
				Message = message,
			};
		}

		public void ApplyEdit( ChatMessage editedMessage )
		{
			Message = editedMessage.Message;
		}
	}

	enum ChatMessageType
	{
		None,

		Incoming,
		Outgoing,
	}
}
