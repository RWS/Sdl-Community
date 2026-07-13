using System;
using NLog;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Drives a correction session: launches a tool, then answers every callback Antidote pushes by
    /// delegating to an <see cref="IWordProcessorAgent"/> and writing framed JSON responses back through
    /// the transport. This is the protocol brain (C# port of Druide's <c>ConnectixAgent</c>) and is
    /// transport/editor agnostic so it can be unit tested with fakes.
    /// </summary>
    public sealed class ConnectixAgent : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectixTransport _transport;
        private readonly IWordProcessorAgent _agent;
        private readonly FrameAssembler _assembler = new FrameAssembler();

        public ConnectixAgent(IConnectixTransport transport, IWordProcessorAgent agent)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            _transport.FrameReceived += OnFrameReceived;
        }

        public void Connect()
        {
            if (!_transport.IsOpen)
                _transport.Open();
        }

        /// <summary>Launches the given <see cref="AntidoteTool"/> id.</summary>
        public void Launch(string tool)
        {
            SendMessage(new LaunchToolMessage { ToolApi = tool });
        }

        private void OnFrameReceived(string frameJson)
        {
            try
            {
                var frame = AntidoteJson.Deserialize<Frame>(frameJson);
                var complete = _assembler.Add(frame);
                if (complete != null)
                    ProcessMessage(complete);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to process incoming Antidote frame.");
            }
        }

        // Internal so unit tests can feed a fully assembled message without faking framing.
        internal void ProcessMessage(string messageJson)
        {
            var message = AntidoteJson.Deserialize<IncomingMessage>(messageJson);
            if (message?.Message == null)
                return;

            switch (message.Message)
            {
                case IncomingMessageType.Init:
                    HandleInit(message);
                    break;
                case IncomingMessageType.DocumentPath:
                    HandleDocumentPath(message);
                    break;
                case IncomingMessageType.DocIsAvailable:
                    HandleDocIsAvailable(message);
                    break;
                case IncomingMessageType.GetTextZones:
                    HandleGetTextZones(message);
                    break;
                case IncomingMessageType.AllowEdit:
                    HandleAllowEdit(message);
                    break;
                case IncomingMessageType.Replace:
                    HandleReplace(message);
                    break;
                case IncomingMessageType.Select:
                    HandleSelect(message);
                    break;
                case IncomingMessageType.ReturnToDocument:
                    _agent.ReturnToDocument();
                    break;
                case IncomingMessageType.NewCorrectionMemory:
                case IncomingMessageType.Send:
                case IncomingMessageType.AntiOopsResponse:
                    // No response required and not used by this plugin.
                    break;
                default:
                    Logger.Warn("Unhandled Antidote message '{0}'.", message.Message);
                    break;
            }
        }

        private void HandleInit(IncomingMessage message)
        {
            var config = _agent.Configuration() ?? new WordProcessorConfiguration();
            var response = new InitResponse
            {
                IdMessage = message.IdMessage,
                DocumentTitle = "Active document",
                CarriageReturn = config.CarriageReturn,
                CacheIdType = config.CacheIdType,
                AllowCarriageReturn = config.AllowCarriageReturn,
                AllowNbSpace = config.AllowNbSpace,
                AllowThinSpace = config.AllowThinSpace,
                AllowSending = config.AllowSending,
                ReplaceWithoutSelection = config.ReplaceWithoutSelection,
                CorrectionMemory = config.CorrectionMemory,
                ActiveMarkup = config.ActiveMarkup
            };
            SendMessage(response);
        }

        private void HandleDocumentPath(IncomingMessage message)
        {
            SendMessage(new DocumentPathResponse
            {
                IdMessage = message.IdMessage,
                Data = _agent.DocumentPath()
            });
        }

        private void HandleDocIsAvailable(IncomingMessage message)
        {
            SendMessage(new DocIsAvailableResponse
            {
                IdMessage = message.IdMessage,
                Donnees = _agent.IsDocumentAvailable()
            });
        }

        private void HandleGetTextZones(IncomingMessage message)
        {
            var forActiveSelection = ReadForActiveSelection(message);
            SendMessage(new TextZonesResponse
            {
                IdMessage = message.IdMessage,
                Data = _agent.ZonesToCorrect(forActiveSelection)
            });
        }

        private void HandleAllowEdit(IncomingMessage message)
        {
            var parameters = message.Data?.ToObject<AllowEditParams>() ?? new AllowEditParams();
            SendMessage(new BoolDataResponse
            {
                IdMessage = message.IdMessage,
                Data = _agent.AllowEdit(parameters)
            });
        }

        private void HandleReplace(IncomingMessage message)
        {
            var parameters = message.Data?.ToObject<ReplaceParams>() ?? new ReplaceParams();
            SendMessage(new BoolDataResponse
            {
                IdMessage = message.IdMessage,
                Data = _agent.Replace(parameters)
            });
        }

        private void HandleSelect(IncomingMessage message)
        {
            var parameters = message.Data?.ToObject<SelectParams>() ?? new SelectParams();
            _agent.Select(parameters);
        }

        private static bool ReadForActiveSelection(IncomingMessage message)
        {
            if (message.ForActiveSelection.HasValue)
                return message.ForActiveSelection.Value;

            var fromData = message.Data?["forActiveSelection"];
            return fromData != null && fromData.ToObject<bool>();
        }

        private void SendMessage(object message)
        {
            var frame = new Frame
            {
                IdFrame = 0,
                TotalFrame = 1,
                Data = AntidoteJson.Serialize(message)
            };
            _transport.Send(AntidoteJson.Serialize(frame));
        }

        public void Dispose()
        {
            _transport.FrameReceived -= OnFrameReceived;
            _transport.Close();
        }
    }
}
