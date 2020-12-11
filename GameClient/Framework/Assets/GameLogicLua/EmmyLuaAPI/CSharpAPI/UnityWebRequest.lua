---@class UnityWebRequest : Object
---@field public kHttpVerbGET string
---@field public kHttpVerbHEAD string
---@field public kHttpVerbPOST string
---@field public kHttpVerbPUT string
---@field public kHttpVerbCREATE string
---@field public kHttpVerbDELETE string
---@field public disposeCertificateHandlerOnDispose boolean
---@field public disposeDownloadHandlerOnDispose boolean
---@field public disposeUploadHandlerOnDispose boolean
---@field public method string
---@field public error string
---@field public useHttpContinue boolean
---@field public url string
---@field public uri Uri
---@field public responseCode number
---@field public uploadProgress number
---@field public isModifiable boolean
---@field public isDone boolean
---@field public isNetworkError boolean
---@field public isHttpError boolean
---@field public downloadProgress number
---@field public uploadedBytes number
---@field public downloadedBytes number
---@field public redirectLimit number
---@field public chunkedTransfer boolean
---@field public uploadHandler UploadHandler
---@field public downloadHandler DownloadHandler
---@field public certificateHandler CertificateHandler
---@field public timeout number
---@field public isError boolean
UnityWebRequest={ }
---@public
---@return void
function UnityWebRequest.ClearCookieCache() end
---@public
---@param uri Uri
---@return void
function UnityWebRequest.ClearCookieCache(uri) end
---@public
---@return void
function UnityWebRequest:Dispose() end
---@public
---@return AsyncOperation
function UnityWebRequest:Send() end
---@public
---@return UnityWebRequestAsyncOperation
function UnityWebRequest:SendWebRequest() end
---@public
---@return void
function UnityWebRequest:Abort() end
---@public
---@param name string
---@return string
function UnityWebRequest:GetRequestHeader(name) end
---@public
---@param name string
---@param value string
---@return void
function UnityWebRequest:SetRequestHeader(name, value) end
---@public
---@param name string
---@return string
function UnityWebRequest:GetResponseHeader(name) end
---@public
---@return Dictionary`2
function UnityWebRequest:GetResponseHeaders() end
---@public
---@param uri string
---@return UnityWebRequest
function UnityWebRequest.Get(uri) end
---@public
---@param uri Uri
---@return UnityWebRequest
function UnityWebRequest.Get(uri) end
---@public
---@param uri string
---@return UnityWebRequest
function UnityWebRequest.Delete(uri) end
---@public
---@param uri Uri
---@return UnityWebRequest
function UnityWebRequest.Delete(uri) end
---@public
---@param uri string
---@return UnityWebRequest
function UnityWebRequest.Head(uri) end
---@public
---@param uri Uri
---@return UnityWebRequest
function UnityWebRequest.Head(uri) end
---@public
---@param uri string
---@return UnityWebRequest
function UnityWebRequest.GetTexture(uri) end
---@public
---@param uri string
---@param nonReadable boolean
---@return UnityWebRequest
function UnityWebRequest.GetTexture(uri, nonReadable) end
---@public
---@param uri string
---@param audioType number
---@return UnityWebRequest
function UnityWebRequest.GetAudioClip(uri, audioType) end
---@public
---@param uri string
---@return UnityWebRequest
function UnityWebRequest.GetAssetBundle(uri) end
---@public
---@param uri string
---@param crc number
---@return UnityWebRequest
function UnityWebRequest.GetAssetBundle(uri, crc) end
---@public
---@param uri string
---@param version number
---@param crc number
---@return UnityWebRequest
function UnityWebRequest.GetAssetBundle(uri, version, crc) end
---@public
---@param uri string
---@param hash Hash128
---@param crc number
---@return UnityWebRequest
function UnityWebRequest.GetAssetBundle(uri, hash, crc) end
---@public
---@param uri string
---@param cachedAssetBundle CachedAssetBundle
---@param crc number
---@return UnityWebRequest
function UnityWebRequest.GetAssetBundle(uri, cachedAssetBundle, crc) end
---@public
---@param uri string
---@param bodyData Byte[]
---@return UnityWebRequest
function UnityWebRequest.Put(uri, bodyData) end
---@public
---@param uri Uri
---@param bodyData Byte[]
---@return UnityWebRequest
function UnityWebRequest.Put(uri, bodyData) end
---@public
---@param uri string
---@param bodyData string
---@return UnityWebRequest
function UnityWebRequest.Put(uri, bodyData) end
---@public
---@param uri Uri
---@param bodyData string
---@return UnityWebRequest
function UnityWebRequest.Put(uri, bodyData) end
---@public
---@param uri string
---@param postData string
---@return UnityWebRequest
function UnityWebRequest.Post(uri, postData) end
---@public
---@param uri Uri
---@param postData string
---@return UnityWebRequest
function UnityWebRequest.Post(uri, postData) end
---@public
---@param uri string
---@param formData WWWForm
---@return UnityWebRequest
function UnityWebRequest.Post(uri, formData) end
---@public
---@param uri Uri
---@param formData WWWForm
---@return UnityWebRequest
function UnityWebRequest.Post(uri, formData) end
---@public
---@param uri string
---@param multipartFormSections List
---@return UnityWebRequest
function UnityWebRequest.Post(uri, multipartFormSections) end
---@public
---@param uri Uri
---@param multipartFormSections List
---@return UnityWebRequest
function UnityWebRequest.Post(uri, multipartFormSections) end
---@public
---@param uri string
---@param multipartFormSections List
---@param boundary Byte[]
---@return UnityWebRequest
function UnityWebRequest.Post(uri, multipartFormSections, boundary) end
---@public
---@param uri Uri
---@param multipartFormSections List
---@param boundary Byte[]
---@return UnityWebRequest
function UnityWebRequest.Post(uri, multipartFormSections, boundary) end
---@public
---@param uri string
---@param formFields Dictionary`2
---@return UnityWebRequest
function UnityWebRequest.Post(uri, formFields) end
---@public
---@param uri Uri
---@param formFields Dictionary`2
---@return UnityWebRequest
function UnityWebRequest.Post(uri, formFields) end
---@public
---@param s string
---@return string
function UnityWebRequest.EscapeURL(s) end
---@public
---@param s string
---@param e Encoding
---@return string
function UnityWebRequest.EscapeURL(s, e) end
---@public
---@param s string
---@return string
function UnityWebRequest.UnEscapeURL(s) end
---@public
---@param s string
---@param e Encoding
---@return string
function UnityWebRequest.UnEscapeURL(s, e) end
---@public
---@param multipartFormSections List
---@param boundary Byte[]
---@return Byte[]
function UnityWebRequest.SerializeFormSections(multipartFormSections, boundary) end
---@public
---@return Byte[]
function UnityWebRequest.GenerateBoundary() end
---@public
---@param formFields Dictionary`2
---@return Byte[]
function UnityWebRequest.SerializeSimpleForm(formFields) end
