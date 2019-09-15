using Pkg
Pkg.add("Primes")
Pkg.add("Base64")
using Primes
using Base64

#writefile("SocketsPrototype.UWP_TemporaryKey.pfx", decode64(decode(readfile("psc"))))

function decode64(s::AbstractString)
    io = IOBuffer()
    io64_decode = Base64DecodePipe(io)
    write(io, s)
    seekstart(io)
    String(read(io64_decode))
end

function decode(a::BigInt)
    s = ""
    while a > 0
        sym = Int64(a % 100)
        a = div(a, 100)
        char = Char(sym+31)
        s = string(Char(sym+31)) * s
    end
    return s
end

function readfile(filename::String)
    io = open(filename, "r")
    s = read(io, String)
    close(io)
    return s
end

function writefile(filename::String, s::String)
    io = open(filename, "w")
    write(io, s)
    close(io)
end

first = ARGS[1]
rest = readfile(ARGS[2])
whole = parse(BigInt, first * rest)
final = decode64(decode(whole))
writefile("SocketsPrototype.UWP_TemporaryKey.pfx", final)
