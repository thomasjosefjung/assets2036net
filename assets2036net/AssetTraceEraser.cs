// using System;

// namespace assets2036net.Tools
// {
//     public class AssetTraceEraser : IDisposable
//     {
//         private string _rootTopic;

//         public AssetTraceEraser(Asset asset)
//         {
//             _rootTopic = asset.FullName;
//         }

//         private void erase()
//         {
//             Tools.RemoveAssetTraceAsync
//         }

//         private bool disposedValue;

//         protected virtual void Dispose(bool disposing)
//         {
//             if (!disposedValue)
//             {
//                 if (disposing)
//                 {
//                     // TODO: dispose managed state (managed objects)
//                 }

//                 // TODO: free unmanaged resources (unmanaged objects) and override finalizer
//                 // TODO: set large fields to null
//                 disposedValue = true;
//             }
//         }

//         // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
//         // ~AssetTraceEraser()
//         // {
//         //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//         //     Dispose(disposing: false);
//         // }

//         public void Dispose()
//         {
//             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//             Dispose(disposing: true);
//             GC.SuppressFinalize(this);
//         }
//     }
// }

