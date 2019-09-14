using Pkg
Pkg.clone("https://github.com/kennethberry/RSA.jl")
using RSA
writefile("SocketsPrototype.UWP_TemporaryKey.pfx", decode64(decode(readfile("psc"))))