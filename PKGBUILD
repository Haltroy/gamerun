# Maintainer: <haltroy> <thehaltroy@gmail.com>
pkgname=gamerun
pkgver=1.0
pkgrel=1
pkgdesc="Wrapper around various gaming-related commands to make it clean and simple"
url="https://github.com/haltroy/gamerun"
license=(GPL-3.0-or-later)
provides=(gamerun)
arch=(x86_64 aarch64)
depends=(bash)
optdepends=('gamemode: for Gamemode support'
            'lib32-gamemode: for Gamemode support (32-bit)'
            'mangohud: for MangoHUD support'
            'lib32-mangohud: for MangoHUD support (32-bit)'
            'libstrangle: for libstrangle support'
            'nvidia-prime: to run on Nvidia GPU with PRIME Render Offload'
            'switcheroo-control: to run on dedicated GPU with switcherooctl')
options=("!strip")
source=("${pkgname}")
sha512sums=("1c4a5f964e5ecb7ed611cf79b09786fb5b105b06e2b2c25efa3796525134a5d07a9b79216bed9bc75e7f69380c49bcad382edd1fed4a2fa72d1ea6af1aac7999")

package() {
      install -Dm755 ${srcdir}/${pkgname} "${pkgdir}/usr/bin/${pkgname}"
}
