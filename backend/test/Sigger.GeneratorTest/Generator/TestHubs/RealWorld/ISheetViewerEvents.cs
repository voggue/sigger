using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public interface ISheetViewerEvents
{
    Task OnChangeSession(SessionId sessionId);
}