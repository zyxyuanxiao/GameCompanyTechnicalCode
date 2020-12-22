#include <assert.h>

#include "GData.h"

#include "MemoryUsage.h" 
#include <cstring>
#include <sstream>

static char LogBuffer[1024] = {0};
void Log(std::string msg)
{
	if(!ConfigManager::Instance().GetLogFunc())
		return;
	std::memset(LogBuffer,0, 1024);
	memcpy(LogBuffer, msg.c_str(), msg.size());
	ConfigManager::Instance().GetLogFunc()(LogBuffer);
    std::memset(LogBuffer,0, 1024);
	//std::cout << msg.c_str() << std::endl; //GCTODO:替换为unity接口
}

byte ReadByte(const void *data, int offset) {
	if (data == NULL) {
		return 0;
	}
	return *((const char*)data + offset);
}

class DataReader
{
public:
	explicit DataReader(const byte *data, int size)
		:m_data(data),
		m_size(size),
		m_offset(0)
	{
	}

	byte ReadUInt8() {
		assert(m_size == 0 || m_offset <= m_size - 1);
		uint8_t val = *(m_data + m_offset);
		m_offset += 1;
		return val;
	}

	std::uint16_t ReadUInt16() {
		assert(m_size == 0 || m_offset <= m_size - 2);
		std::uint16_t val = ntoh(*(std::uint16_t*)(m_data + m_offset));
		m_offset += 2;
		return val;
	}

	std::uint32_t ReadUInt32() {
		assert(m_size == 0 || m_offset <= m_size - 4);
		std::uint32_t val = ntoh(*(std::uint32_t*)(m_data + m_offset));
		m_offset += 4;
		return val;
	}

	std::int32_t ReadSInt32() {
		assert(m_size == 0 || m_offset <= m_size - 4);
		std::int32_t val = ntoh(*(std::uint32_t*)(m_data + m_offset));
		m_offset += 4;
		return val;
	}

	std::uint64_t ReadUInt64() {
		assert(m_size == 0 || m_offset <= m_size - 8);
		std::uint64_t val = ntoh(*(std::uint64_t*)(m_data + m_offset));
		m_offset += 8;
		return val;
	}

	std::int64_t ReadSInt64() {
		assert(m_size == 0 || m_offset <= m_size - 8);
		std::int64_t val = ntoh(*(std::uint64_t*)(m_data + m_offset));
		m_offset += 8;
		return val;
	}

	float ReadFloat() {
        assert(m_offset <= m_size - 8);
        std::int64_t val = ntoh(*(std::uint64_t*)(m_data + m_offset));
        m_offset += 8;
        return val;
    }

	int GetOffset() const { return m_offset; }

	int SetOffset(int newOffset) {
		assert(newOffset >= 0 && (m_size == 0 || newOffset <= m_size));
		int pre = m_offset;
		m_offset = newOffset;
		return pre;
	}

private:
	const byte *m_data;
	int m_size;
	int m_offset = 0;
};


/*
	从一个结构体指针中获取指定变长属性index的地址, index从0开始
*/
const byte *GetVariableAttributePtr(const byte *data, int attributeIndex)
{
	if(data== NULL || attributeIndex < 0)
		return NULL;

	DataReader dr(data, 0);
	uint fixedLength = dr.ReadUInt16();
	uint offset1 = 2 + fixedLength;
	dr.SetOffset(offset1);
	byte optionalLength = dr.ReadUInt8();
	uint offset2 = offset1 + 1 + optionalLength;
	dr.SetOffset(offset2);
	int attributeCount = dr.ReadUInt16();

	if(attributeIndex >= attributeCount){
		return NULL;
	}

	dr.SetOffset(offset2 + 2 + attributeIndex * 2);
	uint relativeOffset = dr.ReadUInt16();
	return data + offset2 + 2 + 2 * attributeCount + relativeOffset;
}

const byte *GetRepeatedStructElementPtr(const byte *repeatedStructPtr, int index)
{
	if(repeatedStructPtr == NULL || index < 0)
		return NULL;

	DataReader dr(repeatedStructPtr, 0);
	uint elementCount = dr.ReadUInt16();
	if(index >= elementCount)
		return NULL;

	dr.SetOffset(2 + 2 * index);
	uint offset = dr.ReadUInt16();
	return repeatedStructPtr + offset;
}


///index从0开始
bool IsEmptyOptional(const byte *dataPtr, uint optionalIndex){
	if(!dataPtr){
		return false;
	}

	DataReader dr(dataPtr, 0);
	uint fixedPartSize = dr.ReadUInt16();
	const byte *offsetData = dataPtr + fixedPartSize + 3;
	int byteIndex = optionalIndex / 8;
	int bitRelativeOffset = 7 - optionalIndex % 8;
	byte val = offsetData[byteIndex];
	return val & (0x1 << bitRelativeOffset);
}

Config::Config(const std::string & name)
	:m_configName(name),
	m_rawDataOffsetInfo(NULL),
	m_rawData(NULL),
	m_rawDataSize(0),
	m_entryCount(0)
{

}

Config::~Config()
{
	if(m_rawDataOffsetInfo != NULL)
		free(m_rawDataOffsetInfo);

	if (m_rawData != NULL)
		free(m_rawData);

	for (auto &iter : m_keyLookupTable) {
		if (iter.second.mapPtr != NULL) {
			free(iter.second.mapPtr);
		}
	}
}

byte *Config::GetEntryPtr(int i) const{
	if(i < 0 || i > m_entryCount)
	{
		std::stringstream fmt; 
		fmt << "GetEntryPtr with invalid index: " << i;
		Log(fmt.str());
		return NULL;
	}

	return m_rawData + ReadNumber<std::uint32_t>(m_rawDataOffsetInfo, i * 4);
}

///解析一个excel所对应的二进制数据
static unsigned char BYTES_MAGIC[9] = "gdataqqq";
void Config::ParseConfigData(byte *data, int size)
{
	if (data == NULL || data == NULL) {
		Log("ParseConfigData with null data");
		return;
	}

	if (m_rawData != NULL) {
		Log(m_configName + " duplicated parsing requese");
		return;
	}

	DataReader dr(data, size);
	for(int i = 0 ; i < 8 ; i++)
	{
		if(BYTES_MAGIC[i] != dr.ReadUInt8())
		{
			Log(m_configName + " isn't gdata binary file.");
			return;
		}
	}

	uint version_code = dr.ReadUInt32(); //暂时没用上
	m_entryCount = dr.ReadUInt32();
	uint stringsOffset = dr.ReadUInt32();
	m_stringsSize = dr.ReadUInt32();
	uint data_meta_offset = dr.ReadUInt32();
	uint data_size = dr.ReadUInt32();

	int rawDataOffsetInfoSize = m_entryCount * 4;
	m_rawDataOffsetInfo = (uint*)malloc(rawDataOffsetInfoSize);
	memcpy(m_rawDataOffsetInfo, data + data_meta_offset, rawDataOffsetInfoSize);

	m_rawDataSize = data_size + m_stringsSize;
	m_rawData = (byte*)malloc(m_rawDataSize);
	memcpy(m_rawData, data + data_meta_offset + rawDataOffsetInfoSize, m_rawDataSize);

	byte keyCount = dr.ReadUInt8();
	for (int i = 0; i < keyCount; i++) {
		CreateKey(dr);
	}
	assert(dr.GetOffset() == data_meta_offset);
}

const Config::KeyDesc *Config::GetKeyDesc(const std::string &keyName) const {
	auto iter = m_keyLookupTable.find(keyName);
	if (iter == m_keyLookupTable.end()) {
		return NULL;
	}
	return &(iter->second);
}


const std::vector<byte*> &Config::GetEntries(const std::string &keyName, std::uint32_t keyValue) const
{
	return GetEntriesImpl<std::uint32_t, std::unordered_map<std::uint32_t, std::vector<byte*>>>(keyName, keyValue, Config::E_KEY_TYPE::UINT32);
}

const std::vector<byte*> &Config::GetEntries(const std::string &keyName, std::int32_t keyValue) const
{
	return GetEntriesImpl<std::int32_t, std::unordered_map<std::int32_t, std::vector<byte*>>>(keyName, keyValue, Config::E_KEY_TYPE::SINT32);
}

const std::vector<byte*> &Config::GetEntries(const std::string &keyName, const std::string &keyValue) const
{
	return GetEntriesImpl<std::string, std::unordered_map<std::string, std::vector<byte*>>>(keyName, keyValue, Config::E_KEY_TYPE::STRING);
}

const byte *Config::GetEntry(const std::string &keyName, std::uint32_t keyValue) const {
	return GetEntryImpl<std::uint32_t, std::unordered_map<std::uint32_t, byte*>>(keyName, keyValue, Config::E_KEY_TYPE::UINT32);
}

const byte *Config::GetEntry(const std::string &keyName, std::int32_t keyValue) const {
	return GetEntryImpl<std::int32_t, std::unordered_map<std::int32_t, byte*>>(keyName, keyValue, Config::E_KEY_TYPE::SINT32);
}

const byte *Config::GetEntry(const std::string &keyName, const std::string &keyValue) const {
	return GetEntryImpl<std::string, std::unordered_map<std::string, byte*>>(keyName, keyValue, Config::E_KEY_TYPE::STRING);
}


void* Config::GetMapPtr(std::string keyName, Config::E_KEY_TYPE keyType, bool unique) const
{
	auto iter = m_keyLookupTable.find(keyName);
	if (iter == m_keyLookupTable.end()) {
		Log(m_configName + " key: " + keyName + " does not exist");
		return NULL;
	}

	if (iter->second.unique != unique)
	{
		Log(keyName + (unique ? " is not " : " is ") + " unique key");
		return NULL;
	}

	if (iter->second.keyType != keyType)
	{
		Log(keyName + " has keyType: " + std::to_string((int)(iter->second.keyType)) + " but request with keyType: " + std::to_string((int)keyType));
		return NULL;
	}

	return iter->second.mapPtr;
}

void Config::CreateKey(DataReader &dr)
{
	uint offset = dr.ReadUInt32();
	std::string keyName = (const char*)(m_rawData + offset);
	byte keyType = dr.ReadUInt8();

	bool unique = (keyType & 0x80) == 0x80;
	bool optional = (keyType & 0x40) == 0x40;
	E_KEY_TYPE ek = (E_KEY_TYPE)(keyType & 0xF);
	switch (ek)
	{
	case Config::E_KEY_TYPE::UINT32:
		if (unique)
			CreateUniqueUInt32Key(dr, keyName, optional);
		else
			CreateMultiUInt32Key(dr, keyName);
		break;
	case Config::E_KEY_TYPE::SINT32:
		if (unique)
			CreateUniqueInt32Key(dr, keyName, optional);
		else
			CreateMultiInt32Key(dr, keyName);
		break;
	case Config::E_KEY_TYPE::STRING:
		if (unique)
			CreateUniqueStringKey(dr, keyName, optional);
		else
			CreateMultiStringKey(dr, keyName);
		break;
	}
}

void Config::CreateMultiStringKey(DataReader &dr, const std::string &keyName) {
	std::unordered_map<std::string, std::vector<byte*>> *map = new std::unordered_map<std::string, std::vector<byte*>>();

	for (uint i = 0; i < m_entryCount; i++) {
		uint string_offset = dr.ReadUInt32();
		if(string_offset == 0)
			continue;

		std::string key = (const char*)(m_rawData + string_offset);
		byte *pos = GetEntryPtr(i);
		auto iter = map->find(key);
		if (iter != map->end()) {
			iter->second.push_back(pos);
		}
		else {
			std::vector<byte*> v;
			v.push_back(pos);
			map->insert(std::make_pair(key, v));
		}
	}

	KeyDesc desc;
	desc.keyType = Config::E_KEY_TYPE::STRING;
	desc.unique = false;
	desc.mapPtr = (void*)map;
	m_keyLookupTable.insert(std::make_pair(keyName, desc));
}

void Config::CreateUniqueStringKey(DataReader &dr, const std::string &keyName, bool optional) {
	std::unordered_map<std::string, byte*> *map = new std::unordered_map<std::string, byte*>();

	for (uint i = 0; i < m_entryCount; i++) {
		uint string_offset = dr.ReadUInt32();
		if(string_offset == 0 )
			continue;

		std::string key = (const char*)(m_rawData + string_offset);
		byte *pos = GetEntryPtr(i);
		map->insert(std::make_pair(key, pos));
	}

	KeyDesc desc;
	desc.keyType = Config::E_KEY_TYPE::STRING;
	desc.unique = true;
	desc.mapPtr = (void*)map;
	desc.optional = optional;
	m_keyLookupTable.insert(std::make_pair(keyName, desc));
}

void Config::CreateMultiUInt32Key(DataReader &dr, const std::string &keyName) {
	std::unordered_map<std::uint32_t, std::vector<byte*>> *map = new std::unordered_map<std::uint32_t, std::vector<byte*>>();

	for (uint i = 0; i < m_entryCount; i++) {
		std::uint32_t key = dr.ReadUInt32();
		byte *pos = GetEntryPtr(i);
		auto iter = map->find(key);
		if (iter != map->end()) {
			iter->second.push_back(pos);
		}
		else {
			std::vector<byte*> v;
			v.push_back(pos);
			map->insert(std::make_pair(key, v));
		}
	}

	KeyDesc desc;
	desc.keyType = Config::E_KEY_TYPE::UINT32;
	desc.unique = false;
	desc.mapPtr = (void*)map;
	m_keyLookupTable.insert(std::make_pair(keyName, desc));
}

void Config::CreateUniqueUInt32Key(DataReader &dr, const std::string &keyName, bool optional) {
	std::unordered_map<std::uint32_t, byte*> *map = new std::unordered_map<std::uint32_t, byte*>();

	for (uint i = 0; i < m_entryCount; i++) {
		std::uint32_t key = dr.ReadUInt32();
		byte *pos = GetEntryPtr(i);
		map->insert(std::make_pair(key, pos));
	}

	KeyDesc desc;
	desc.keyType = Config::E_KEY_TYPE::UINT32;
	desc.unique = true;
	desc.mapPtr = (void*)map;
	desc.optional = optional;
	m_keyLookupTable.insert(std::make_pair(keyName, desc));
}

void Config::CreateMultiInt32Key(DataReader &dr, const std::string &keyName) {
	std::unordered_map<std::int32_t, std::vector<byte*>> *map = new std::unordered_map<std::int32_t, std::vector<byte*>>();

	for (uint i = 0; i < m_entryCount; i++) {
		std::int32_t key = dr.ReadSInt32();
		byte *pos = GetEntryPtr(i);
		auto iter = map->find(key);
		if (iter != map->end()) {
			iter->second.push_back(pos);
		}
		else {
			std::vector<byte*> v;
			v.push_back(pos);
			map->insert(std::make_pair(key, v));
		}
	}

	KeyDesc desc;
	desc.keyType = Config::E_KEY_TYPE::SINT32;
	desc.unique = false;
	desc.mapPtr = (void*)map;
	m_keyLookupTable.insert(std::make_pair(keyName, desc));
}

void Config::CreateUniqueInt32Key(DataReader &dr, std::string keyName, bool optional) {
	std::unordered_map<std::int32_t, byte*> *map = new std::unordered_map<std::int32_t, byte*>();

	for (uint i = 0; i < m_entryCount; i++) {
		std::int32_t key = dr.ReadSInt32();
		byte *pos = GetEntryPtr(i);
		map->insert(std::make_pair(key, pos));
	}

	KeyDesc desc;
	desc.keyType = Config::E_KEY_TYPE::SINT32;
	desc.unique = true;
	desc.mapPtr = (void*)map;
	desc.optional = optional;
	m_keyLookupTable.insert(std::make_pair(keyName, desc));
}

///////////////////////////////////////

ConfigManager &ConfigManager::Instance() {
	if(s_inst == NULL){
		s_inst = new ConfigManager();
	}
	return *s_inst;
}

void ConfigManager::Release(){
	if(s_inst != NULL){
		delete s_inst;
		s_inst = NULL;
		memory_usage_in_bytes = 0;
	}
}

void ConfigManager::ParseConfigFile(const std::string &configName, byte *data, int size) {
	if (size <= 0) {
		Log("parsing configname: " + configName + "with invalid data size");
		return;
	}

	if (!data) {
		Log("parsing configname: " + configName + "with null data");
		return;
	}

	auto iter = m_allConfigs.find(configName);
	if (iter != m_allConfigs.end()) {
		Log("duplicate parsing request on configname: " + configName);
		return;
	}

	enable_memory_trace = true;
	Config *c = new Config(configName);
	c->ParseConfigData((byte*)data, size);
	m_allConfigs.insert(std::make_pair(configName, c));
	enable_memory_trace = false;
}

void ConfigManager::ParseMergedConfigFile(byte *mergedData, int size) {
	//GCTODO:
}

static const int NAME_BUFFER_LEN = 1024;
static char ConfigPtrNameBuffer[NAME_BUFFER_LEN];
const Config *ConfigManager::GetConfig(const std::string &name) 
{
	auto iter = m_allConfigs.find(name);
	if (iter == m_allConfigs.end()) {
		if(!m_readConfigFunc){
			return NULL;
		}

		std::memset((void*)(ConfigPtrNameBuffer), 0, NAME_BUFFER_LEN);
		memcpy((void*)(ConfigPtrNameBuffer), name.c_str(), name.length());

		int size = 0;
		m_lazyReadConfigName = ConfigPtrNameBuffer;
		m_readConfigFunc(ConfigPtrNameBuffer);

		iter = m_allConfigs.find(m_lazyReadConfigName);
		if(iter == m_allConfigs.end())
			return NULL;
	}

	return iter->second;
}

void ConfigManager::OnReadConfig(void *bytes, int size){
	ParseConfigFile(m_lazyReadConfigName, (byte*)bytes, size);
}


ConfigManager::~ConfigManager()
{
	for (auto &iter : m_allConfigs) {
		delete(iter.second);
	}
}

void ConfigManager::SetReadConfigFunc(ReadConfigFunc func){
	m_readConfigFunc = func;
}

std::uint32_t ConfigManager::GetMemoryUsage() const {
	std::uint32_t bytes = 0;
	for (auto &iter : m_allConfigs) {
		bytes += iter.second->RawDataSize();
		bytes += iter.second->GetEntryCount() * 4;
	}
	bytes += memory_usage_in_bytes;
	return bytes;
}

ConfigManager *ConfigManager::s_inst = NULL;
