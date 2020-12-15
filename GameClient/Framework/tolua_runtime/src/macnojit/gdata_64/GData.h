#ifndef GDATA_H
#define GDATA_H

#include <string>
#include <unordered_map>
#include <set>
#include <vector>

typedef void(*LogFunc)(void *msg);
typedef void(*ReadConfigFunc)(const char *config_name);

using uint = unsigned int;
using byte = std::uint8_t;

void Log(std::string msg);

class DataReader;

static std::vector<byte*> dummyVector;

static union {
	uint32_t i;
	char c[4];
} bint = { 0x01020304 };

inline bool is_big_endian(void) {
	return bint.c[0] == 1;
}

template <typename T>
inline T ntoh(const T &input) { 
	if (is_big_endian())
	{
		return input;
	}

	T output(0);
	const std::size_t size = sizeof(T);
	byte *data = reinterpret_cast<byte *>(&output);

	for (std::size_t i = 0; i < size; i++) {
		data[i] = (byte)(input >> ((size - i - 1) * 8));
	}

	return output;
}

template<typename T>
T ReadNumber(const void *data, int offset) {
	if (data == NULL) {
		return T{};
	}
	const char *pos = (const char *)(((char*)data) + offset);
	T val = *(T*)(pos);
	return ntoh(val);
}

byte ReadByte(const void *data, int offset);

/*
	从一个结构体指针中获取指定变长属性index的地址, index从0开始
*/
const byte *GetVariableAttributePtr(const byte *data, int attributeIndex);

/*
	从一个结构体数组中获取指定index的结构体指针, index从0开始
*/
const byte *GetRepeatedStructElementPtr(const byte *repeatedStructPtr, int index);

/*
	判断struct中 optional是否为空
	ndex从0开始
*/
bool IsEmptyOptional(const byte *dataPtr, uint optional_index) ;

class Config {
public:
	enum E_KEY_TYPE {
		UINT32 = 1,
		SINT32 = 2,
		STRING = 3,
	};

	struct KeyDesc {
		E_KEY_TYPE keyType;
		bool unique;
		void *mapPtr;
		bool optional;
	};

	explicit Config(const std::string & name);

	~Config();

	void ParseConfigData(byte *data, int size);
	

	inline std::uint32_t RawDataSize() const { return m_rawDataSize; }

	const KeyDesc *GetKeyDesc(const std::string &keyName) const;

	byte *GetEntryPtr(int i) const;

	inline int GetEntryCount() const { return m_entryCount; }

	template<typename TKey, typename TUniqueMap, typename TMultiMap>
	std::vector<TKey> GetALlKeys(const std::string &keyName, Config::E_KEY_TYPE keyType) const {
		const KeyDesc *desc = GetKeyDesc(keyName);
		if (!desc) {
			return std::vector<TKey>();
		}

		if (desc->keyType != keyType) {
			return std::vector<TKey>();
		}

		std::vector<TKey> result;
		if (desc->unique) {
			for (auto iter : *(TUniqueMap*)(desc->mapPtr)) {
				result.push_back(iter.first);
			}
		}
		else {
			for (auto &iter : *(TMultiMap*)(desc->mapPtr)) {
				result.push_back(iter.first);
			}
		}
		return result;
	}

	template<typename T1, typename T2>
	std::vector<byte*> GetEntries(const std::string &keyName1, const T1 &keyValue1, const std::string &keyName2, const T2 &keyValue2) const
	{
		const std::vector<byte*> &v1 = GetEntries(keyName1, keyValue1);
		const std::vector<byte*> &v2 = GetEntries(keyName2, keyValue2);

		std::set<byte*> lookup;
		for (byte *b : v2)
			lookup.insert(b);

		std::vector<byte*> result;
		for (byte *b1 : v1) {
			if (lookup.find(b1) != lookup.cend())
				result.push_back(b1);
		}
		return result;
	}

	template<typename T1, typename T2, typename T3>
	std::vector<byte*> GetEntries(const std::string &keyName1, const T1 &keyValue1, const std::string &keyName2, const T2 &keyValue2, const std::string &keyName3, const T3 &keyValue3) const
	{
		const std::vector<byte*> &v1 = GetEntries(keyName1, keyValue1);
		const std::vector<byte*> &v2 = GetEntries(keyName2, keyValue2);
		const std::vector<byte*> &v3 = GetEntries(keyName3, keyValue3);

		std::set<byte*> lookup2;
		std::set<byte*> lookup3;
		for (byte *b : v2)
			lookup2.insert(b);
		for (byte *b : v3)
			lookup3.insert(b);

		std::vector<byte*> result;
		for (byte *b : v1) {
			if (lookup2.find(b) != lookup2.cend() && lookup3.find(b) != lookup3.cend())
				result.push_back(b);
		}
		return result;
	}


	template<typename T1, typename T2>
	byte* GetEntry(const std::string &keyName1, const T1 &keyValue1, const std::string &keyName2, const T2 &keyValue2) const
	{
		const std::vector<byte*> &v1 = GetEntries(keyName1, keyValue1);
		const std::vector<byte*> &v2 = GetEntries(keyName2, keyValue2);

		std::set<byte*> lookup2;
		for (byte *b : v2)
			lookup2.insert(b);

		for (byte *b1 : v1) {
			if (lookup2.find(b1) != lookup2.cend())
				return b1;
		}
		return NULL;
	}

	template<typename T1, typename T2, typename T3>
	byte* GetEntry(const std::string &keyName1, const T1 &keyValue1, const std::string &keyName2, const T2 &keyValue2, const std::string &keyName3, const T3 &keyValue3) const
	{
		const std::vector<byte*> &v1 = GetEntries(keyName1, keyValue1);
		const std::vector<byte*> &v2 = GetEntries(keyName2, keyValue2);
		const std::vector<byte*> &v3 = GetEntries(keyName3, keyValue3);

		std::set<byte*> lookup2;
		std::set<byte*> lookup3;
		for (byte *b : v2)
			lookup2.insert(b);
		for (byte *b : v3)
			lookup3.insert(b);

		for (byte *b1 : v1) {
			if (lookup2.find(b1) != lookup2.cend() && lookup3.find(b1) != lookup3.cend())
				return b1;
		}
		return NULL;
	}

	//template<typename TKeyValue, typename TMap>
	//const std::vector<byte*> &GetEntriesImpl(const std::string &keyName, TKeyValue keyValue, Config::E_KEY_TYPE keyType) const {


	const std::vector<byte*> &GetEntries(const std::string &keyName, std::uint32_t keyValue) const;
	const std::vector<byte*> &GetEntries(const std::string &keyName, std::int32_t keyValue) const;
	const std::vector<byte*> &GetEntries(const std::string &keyName, const std::string &keyValue) const;
	

	const byte *GetEntry(const std::string &keyName, std::uint32_t keyValue) const;
	const byte *GetEntry(const std::string &keyName, std::int32_t keyValue) const;
	const byte *GetEntry(const std::string &keyName, const std::string &keyValue) const;


private:
	template<typename TKeyValue, typename TMap>
	const std::vector<byte*> &GetEntriesImpl(const std::string &keyName, const TKeyValue &keyValue, Config::E_KEY_TYPE keyType) const {
		void *mapPtr = GetMapPtr(keyName, keyType, false);
		if (mapPtr == NULL)
			return dummyVector;

		TMap *map = (TMap *)(mapPtr);
		auto iter = map->find(keyValue);
		if (iter != map->end()) {
			return iter->second;
		}
		return dummyVector;
	}

	template<typename TKeyValue, typename TMap>
	const byte *GetEntryImpl(const std::string &keyName, TKeyValue keyValue, Config::E_KEY_TYPE keyType) const {
		void *mapPtr = GetMapPtr(keyName, keyType, true);
		if (mapPtr == NULL)
			return NULL;

		TMap *map = (TMap *)(mapPtr);
		auto iter = map->find(keyValue);
		if (iter != map->end()) {
			return iter->second;
		}
		return NULL;
	}


	void* GetMapPtr(std::string keyName, Config::E_KEY_TYPE keyType, bool unique) const;
	
	void CreateKey(DataReader &dr);
	
	void CreateMultiStringKey(DataReader &dr, const std::string &keyName);

	void CreateUniqueStringKey(DataReader &dr, const std::string &keyName, bool optional);

	void CreateMultiUInt32Key(DataReader &dr, const std::string &keyName);

	void CreateUniqueUInt32Key(DataReader &dr, const std::string &keyName, bool optional);

	void CreateMultiInt32Key(DataReader &dr, const std::string &keyName);

	void CreateUniqueInt32Key(DataReader &dr, std::string keyName, bool optional);

private:
    const std::string & m_configName;
	std::unordered_map<std::string, KeyDesc> m_keyLookupTable;

	uint *m_rawDataOffsetInfo;
	byte *m_rawData;
	uint m_rawDataSize;
	uint m_stringsSize;

	uint m_entryCount;
};


class ConfigManager {
public:
	static ConfigManager &Instance();
	static void Release();
	~ConfigManager();

	void ParseConfigFile(const std::string &configName, byte *data, int size);

	//GCTODO:
	void ParseMergedConfigFile(byte *mergedData, int size);

	const Config *GetConfig(const std::string &name) ;

	std::uint32_t GetMemoryUsage() const;

	inline void SetLogFunc(LogFunc func){m_logFunc = func;}
	inline LogFunc GetLogFunc() const {return m_logFunc;}

	void SetReadConfigFunc(ReadConfigFunc func);
	void OnReadConfig(void *bytes, int size);

private:
	explicit ConfigManager() = default;
	static ConfigManager *s_inst;
	LogFunc m_logFunc;
	ReadConfigFunc m_readConfigFunc;
	std::string m_lazyReadConfigName;

	std::unordered_map<std::string, Config*> m_allConfigs;
};
#endif
