#include <std/string.pat>
#include <type/guid.pat>

using String = std::string::SizedString16<u32>;

struct FileEntry {
    u32 unk1; // always 0?
    String cloud_filename;
    String local_filename;
    String revision;
    u8 container_id;
    u32 unk2; // always 1?
    type::GUID container_folder;
    u64 unk3; // timestamp?
    u64 unk4; // always 0?
    u32 filesize;
};

struct Index {
    u32 unk1;
    u32 item_count;
    u32 unk2;
    String title;
    u64 unk3; //timestamp?

    u32 unk4;
    String guid;
    u32 unk5; // flags?

    FileEntry files[item_count];
    
    u32 unk6;
};

Index idx @ 0x0;