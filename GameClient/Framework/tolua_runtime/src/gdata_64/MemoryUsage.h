#ifndef MEMORY_USAGE_H
#define MEMORY_USAGE_H

#include <cstdlib>
#include <unordered_map>

// number of objects to create
static size_t n = 0;

// total memory_usage_in_bytes allocated
static size_t memory_usage_in_bytes = 0;

// total number of malloc calls
static size_t allocations = 0;

// total number free calls
static size_t frees = 0;

// static vector for tracking allocation sizes
static std::unordered_map<void*, size_t> sizes;

static bool enable_memory_trace = false;

static void* alloc(size_t size) {
	void* p = std::malloc(size);
	if (enable_memory_trace && p != NULL) {
		// update stats
		enable_memory_trace = false;
		memory_usage_in_bytes += size;
		++allocations;
		sizes[p] = size;
		enable_memory_trace = true;
		// for debugging
		// std::cout << "- allocating " << size << " memory_usage_in_bytes at memory location " << p << std::endl;
	}
	return p;
}

static void dealloc(void* p) {
	if (p == NULL) {
		return;
	}
	// update the stats
	if (enable_memory_trace) {
		enable_memory_trace = false;
		++frees;
		size_t size = 0;
		auto iter = sizes.find(p);
		if (iter != sizes.end()) {
			size = iter->second;
		}
		memory_usage_in_bytes -= size;
		// for debugging
		// std::cout << "- freeing " << size << " memory_usage_in_bytes at memory location " << p << std::endl;
		enable_memory_trace = true;
	}
	std::free(p);
}

void* operator new(size_t size) throw(std::bad_alloc) {
	return alloc(size);
}

void* operator new(size_t size, std::nothrow_t const&) throw() {
	return alloc(size);
}

void* operator new[](size_t size) throw(std::bad_alloc) {
	return alloc(size);
}

void* operator new[](size_t size, std::nothrow_t const&) throw() {
	return alloc(size);
}

void operator delete(void* p) throw() {
	dealloc(p);
}

void operator delete(void* p, std::nothrow_t const&) throw() {
	dealloc(p);
}

void operator delete[](void* p) throw() {
	dealloc(p);
}

void operator delete[](void* p, std::nothrow_t const&) throw() {
	dealloc(p);
}
#endif