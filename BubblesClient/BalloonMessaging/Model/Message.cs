using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Balloons.Messaging.Model
{
    /// <summary>
    /// List of all message types.
    /// </summary>
    public enum MessageType
    {
        // Messages sent by the server
        NewBalloon,
        BalloonContentUpdate,
        NewPlane,

        // Messages sent by the client
        ChangeScreen,
        GetBalloonContent,
        GetBalloonState,

        // Messages sent by both
        PopObject,
        BalloonStateUpdate,

        // Internal messages
        Connected,
        Disconnected,
        RefreshFeed,
        FeedUpdated,
        Callback
    }
    
    /// <summary>
    /// Message that can be sent between a screen and bubble server or within the server.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Identifies the type of the message.
        /// </summary>
        public MessageType Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// Identifies the type of the message.
        /// </summary>
        public string TypeTag
        {
            get { return m_tag; }
        }

        /// <summary>
        /// Object that sent this message.
        /// </summary>
        public object Sender
        {
            get { return m_sender; }
            set { m_sender = value; }
        }
        
        public Message(MessageType type, string tag)
        {
            m_type = type;
            m_tag = tag;
        }
        
        private readonly MessageType m_type;
        private readonly string m_tag;
        private object m_sender;
    }

    public class ObjectMessage : Message
    {
        public string ObjectID
        {
            get { return this.m_objectID; }
        }

        public ObjectMessage(MessageType type, string tag, string objectID)
            : base(type, tag)
        {
            m_objectID = objectID;
        }

        private string m_objectID;
    }

    public class MessageEventArgs : EventArgs
    {
        public Message Message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        public MessageEventArgs(Message message)
        {
            m_message = message;
        }

        private Message m_message;
    }

    #region Messages sent by the server
    public class NewBalloonMessage : ObjectMessage
    {
        public const string Tag = "new-balloon";

        public Direction Direction
        {
            get { return this.m_direction; }
        }

        public float Y
        {
            get { return this.m_y; }
        }

        public Vector2D Velocity
        {
            get { return this.m_velocity; }
        }

        public NewBalloonMessage(string balloonID, Direction direction, float y, Vector2D velocity)
            : base(MessageType.NewBalloon, Tag, balloonID)
        {
            m_direction = direction;
            m_y = y;
            m_velocity = velocity;
        }

        private Direction m_direction;
        private Vector2D m_velocity;
        private float m_y;
    }

    public class NewPlaneMessage : ObjectMessage
    {
        public const string Tag = "new-plane";

        public Direction Direction
        {
            get { return this.m_direction; }
        }

        public PlaneType PlaneType
        {
            get { return this.m_planeType; }
        }

        public float Y
        {
            get { return this.m_y; }
        }

        public Vector2D Velocity
        {
            get { return this.m_velocity; }
        }

        public float Time
        {
            get { return this.m_time; }
        }

        public NewPlaneMessage(string planeID, PlaneType planeType, Direction direction, float y, Vector2D velocity, float time)
            : base(MessageType.NewPlane, Tag, planeID)
        {
            m_direction = direction;
            m_y = y;
            m_velocity = velocity;
            m_planeType = planeType;
            m_time = time;
        }

        private Direction m_direction;
        private PlaneType m_planeType;
        private Vector2D m_velocity;
        private float m_y;
        private float m_time;
    }

    public class BalloonContentUpdateMessage : ObjectMessage
    {
        public const string Tag = "balloon-content-update";

        public BalloonType BalloonType
        {
            get { return this.m_type; }
        }

        public string Label
        {
            get { return this.m_label; }
        }

        public string Content
        {
            get { return this.m_content; }
        }

        public string Url
        {
            get { return this.m_url; }
        }
        
        public string ImageUrl
        {
            get { return this.m_imageUrl;  }
        }

        public BalloonContentUpdateMessage(string balloonID, BalloonType type, string label, string content, string url, string imageURL)
            : base(MessageType.BalloonContentUpdate, Tag, balloonID)
        {
            m_type = type;
            m_label = label;
            m_content = content;
            m_url = url;
            m_imageUrl = imageURL;
        }
        
        public BalloonContentUpdateMessage(Balloon balloon)
            : this(balloon.ID, balloon.Type, balloon.Label, balloon.Content, balloon.Url, balloon.ImageUrl)
        {
        }
        
        public void UpdateContent(Balloon balloon)
        {
            if(balloon == null)
            {
                return;
            }
            balloon.Type = BalloonType;
            balloon.Label = Label;
            balloon.Content = Content;
            balloon.Url = Url;
            balloon.ImageUrl = ImageUrl;
        }

        private BalloonType m_type;
        private string m_label;
        private string m_content;
        private string m_url;
        private string m_imageUrl;
    }
    #endregion

    #region Messages sent by the client
    public class ChangeScreenMessage : ObjectMessage
    {
        public const string Tag = "change-screen";

        public Direction Direction
        {
            get { return this.m_direction; }
        }

        public float Y
        {
            get { return this.m_y; }
        }

        public Vector2D Velocity
        {
            get { return this.m_velocity; }
        }

        public float Time
        {
            get { return this.m_time; }
        }

        public ChangeScreenMessage(string balloonID, Direction direction, float y, Vector2D velocity, float time = 0)
            : base(MessageType.ChangeScreen, Tag, balloonID)
        {
            m_direction = direction;
            m_y = y;
            m_velocity = velocity;
            m_time = time;
        }
        
        private Direction m_direction;
        private Vector2D m_velocity;
        private float m_y;
        private float m_time;
    }

    public class GetBalloonContentMessage : ObjectMessage
    {
        public const string Tag = "get-balloon-content";

        public GetBalloonContentMessage(string balloonID)
            : base(MessageType.GetBalloonContent, Tag, balloonID)
        {
        }
    }

    public class GetBalloonStateMessage : ObjectMessage
    {
        public const string Tag = "get-balloon-state";

        public GetBalloonStateMessage(string balloonID)
            : base(MessageType.GetBalloonState, Tag, balloonID)
        {
        }
    }
    #endregion

    #region Messages sent by both
    public class PopObjectMessage : ObjectMessage
    {
        public const string Tag = "pop-balloon";

        public PopObjectMessage(string balloonID)
            : base(MessageType.PopObject, Tag, balloonID)
        {
        }
    }

    public class BalloonStateUpdateMessage : ObjectMessage
    {
        public const string Tag = "balloon-state-update";

        public OverlayType OverlayType
        {
            get { return this.m_overlayType; }
        }

        public Colour BackgroundColor
        {
            get { return this.m_bgColor; }
        }

        public int Votes
        {
            get { return this.m_votes; }
        }

        public BalloonStateUpdateMessage(string balloonID, OverlayType overlayType, Colour bgColor, int votes)
            : base(MessageType.BalloonStateUpdate, Tag, balloonID)
        {
            m_overlayType = overlayType;
            m_bgColor = bgColor;
            m_votes = votes;
        }
        
        public BalloonStateUpdateMessage(Balloon balloon)
            : this(balloon.ID, balloon.OverlayType, balloon.BackgroundColor, balloon.Votes)
        {
        }
        
        public void UpdateState(Balloon balloon)
        {
            if(balloon == null)
            {
                return;
            }
            balloon.OverlayType = OverlayType;
            balloon.BackgroundColor = BackgroundColor;
            balloon.Votes = Votes;
        }

        private OverlayType m_overlayType;
        private Colour m_bgColor;
        private int m_votes;
    }
    #endregion

    #region Internal messages
    public class ConnectedMessage : Message
    {
        public static readonly string Tag = "connected";

        public Socket Connection
        {
            get { return this.m_socket; }
        }
        
        public ConnectedMessage(Socket socket) : base(MessageType.Connected, Tag)
        {
            m_socket = socket;
        }
        
        private Socket m_socket;
    }
    
    public class DisconnectedMessage : Message
    {
        public static readonly string Tag = "disconnected";

        public DisconnectedMessage() : base(MessageType.Disconnected, Tag)
        {
        }
    }
    
    public class FeedUpdatedMessage : Message
    {
        public static readonly string Tag = "feed-updated";

        public List<FeedContent> FeedItems
        {
            get { return this.m_items; }
        }
        
        public FeedUpdatedMessage(List<FeedContent> items) : base(MessageType.FeedUpdated, Tag)
        {
            m_items = items;
        }
        
        private List<FeedContent> m_items;
    }

    public delegate void GameCallback();

    public class CallbackMessage : Message
    {
        private GameCallback m_callback;

        public GameCallback Callback
        {
            get { return m_callback; }
        }

        public CallbackMessage(GameCallback callback)
            : base(MessageType.Callback, "callback")
        {
            m_callback = callback;
        }
    }
    #endregion
}