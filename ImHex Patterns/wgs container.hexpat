#include <type/guid.pat>

struct Entry {
    char16 name[64];
    type::GUID filename[2];
};

struct Container {
    u32 unk1; // version?
    u32 count;
    
    Entry entries[count];
};

Container container @ 0x0;