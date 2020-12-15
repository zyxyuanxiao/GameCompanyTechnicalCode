#include "GData.h"
#include <cstring>

template<typename TValue>
void* GetDataPtr_OneParam_Impl(const char *config_name, const char *key_name, TValue value) {
	if (!config_name || !key_name) {
		Log("C# GetDataPtr with invalid arg");
		return NULL;
	}

	const Config *configPtr = ConfigManager::Instance().GetConfig(config_name);
	if (!configPtr) {
		Log("C# GetDataPtr cannot find config: " + std::string(config_name));
		return NULL;
	}

	return (void*)(configPtr->GetEntry(key_name, value));
}

template<typename TValue>
void* GetDataPtrs_OneParam_Impl(const char *config_name, const char *key_name, TValue value, int &arraySize) {
	if (!config_name || !key_name) {
		Log("C# GetDataPtr with invalid arg");
		return NULL;
	}

	const Config *configPtr = ConfigManager::Instance().GetConfig(config_name);
	if (!configPtr) {
		Log("C# GetDataPtr cannot find config: " + std::string(config_name));
		return NULL;
	}

	const std::vector<byte*> &v = configPtr->GetEntries(key_name, value);
	arraySize = (int)(v.size());

	if (v.size() == 0) {
		return NULL;
	}
	else {
		return (void*)(&v[0]);
	}
}

extern "C"
{
	extern void GetAllData(const char *config_name, void **ptrArray, int &arraySize) {
		arraySize = 0;
		if(!config_name || !ptrArray){
			return ;
		}
		const Config *config = ConfigManager::Instance().GetConfig(config_name);
		if(!config){
			return ;
		}
		arraySize = config->GetEntryCount();
		for(int i = 0 ; i < arraySize ; i++){
			*(ptrArray + i) = (void*)(config->GetEntryPtr(i));
		}
	}

	extern void ParseConfigData(const char* configName, bool merged, byte* data, int size)
	{
		if (merged) {
			ConfigManager::Instance().ParseMergedConfigFile(data, size);
		}
		else {
			ConfigManager::Instance().ParseConfigFile(configName, data, size);
		}
		return;
	}

	extern std::uint16_t ReadUInt16(byte* data, int offset) {
		return ReadNumber<std::uint16_t>(data, offset);
	}

	extern std::uint32_t ReadUInt32(byte* data, int offset) {
		return ReadNumber<std::uint32_t>(data, offset);
	}

	extern std::int32_t ReadSInt32(byte* data, int offset) {
		return ReadNumber<std::int32_t>(data, offset);
	}

	extern std::uint64_t ReadUInt64(byte* data, int offset) {
		return ReadNumber<std::uint64_t>(data, offset);
	}

	extern std::int64_t ReadSInt64(byte* data, int offset) {
		return ReadNumber<std::int64_t>(data, offset);
	}

	extern bool ReadBool(byte* data, int offset) {
		return ReadByte(data, offset) == 1;
	}

	static const int BUFF_LEN = 65536;
	static char PInvokeBuffer[BUFF_LEN];
	extern const char * ReadString(byte* data, int offset, int &arraySize) {
		if (!data) {
			return NULL;
		}

		std::uint32_t relative_offset = ReadNumber<std::uint32_t>(data, offset);
		if(relative_offset == 0)
			return NULL;
		const char *str = (const char*)(data + relative_offset + offset);
		if (!str) {
			return NULL;
		}
		std::memset((void*)(&PInvokeBuffer[0]), 0, BUFF_LEN);
		int len = strlen(str) > BUFF_LEN ? BUFF_LEN : strlen(str);
		arraySize = len;
		memcpy((void*)(&PInvokeBuffer[0]), str, len);
		return PInvokeBuffer;
	}

	extern void* GetConfigPtr(const char *config_name) {
		if (!config_name) {
			Log("C# GetConfigPtr with null string");
			return NULL;
		}

		const Config *configPtr = ConfigManager::Instance().GetConfig(config_name);
		return (void*)configPtr;
	}

	extern uint GetEntryCount(const char *config_name) {
		if (!config_name) {
			Log("C# GetEntryCount with null string");
			return 0;
		}

		const Config *configPtr = ConfigManager::Instance().GetConfig(config_name);
		if(!configPtr){
			return 0;
		}
		return configPtr->GetEntryCount();
	}

	extern void* GetDataPtr_UInt32(const char *config_name, const char *key_name, std::uint32_t value) {
		return GetDataPtr_OneParam_Impl(config_name, key_name, value);
	}

	extern void* GetDataPtr_SInt32(const char *config_name, const char *key_name, std::int32_t value) {
		return GetDataPtr_OneParam_Impl(config_name, key_name, value);
	}

	extern void* GetDataPtr_String(const char *config_name, const char *key_name, const char *value) {
		return GetDataPtr_OneParam_Impl(config_name, key_name, value);
	}

	extern void* GetDataPtrs_UInt32(const char *config_name, const char *key_name, std::uint32_t value, int &arraySize) {
		return GetDataPtrs_OneParam_Impl(config_name, key_name, value, arraySize);
	}

	extern void* GetDataPtrs_SInt32(const char *config_name, const char *key_name, std::int32_t value, int &arraySize) {
		return GetDataPtrs_OneParam_Impl(config_name, key_name, value, arraySize);
	}

	extern void* GetDataPtrs_String(const char *config_name, const char *key_name, const char *value, int &arraySize) {
		return GetDataPtrs_OneParam_Impl(config_name, key_name, value, arraySize);
	}

	extern void SetLogFunc(LogFunc impl){
		ConfigManager::Instance().SetLogFunc(impl);
	}

	extern void Release(){
		ConfigManager::Instance().Release();
	}

	extern void SetReadConfigFunc(ReadConfigFunc func){
		ConfigManager::Instance().SetReadConfigFunc(func);
	}

	extern void OnReadConfig(void *bytes, int size){
		ConfigManager::Instance().OnReadConfig(bytes, size);
	}

	extern const void* GetVariableAttributePtr_CSharp(const byte *data, int variable_attri_index){
		return GetVariableAttributePtr(data, variable_attri_index);
	}

	extern const void *GetRepeatedStructElementPtr_CSharp(const byte *repeatedStructPtr, int index){
		return GetRepeatedStructElementPtr(repeatedStructPtr, index);
	}

/*
	extern void* GetAllKeys_UInt32(const char *config_name, const char *key_name, int &arraySize){
		return GetKeys_Impl<std::uint32_t>(config_name, key_name, arraySize, Config::E_KEY_TYPE::UINT32);
	}

	extern void* GetAllKeys_SInt32(const char *config_name, const char *key_name, int &arraySize){
		return GetKeys_Impl<std::int32_t>(config_name, key_name, arraySize, Config::E_KEY_TYPE::SINT32);
	}

	extern void* GetAllKeys_String(const char *config_name, const char *key_name, int &arraySize){
		return GetKeys_Impl<std::string>(config_name, key_name, arraySize, Config::E_KEY_TYPE::STRING);
	}
*/
}


