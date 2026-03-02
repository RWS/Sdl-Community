<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format" xmlns:XmlReporting="urn:XmlReporting">

	<!-- Embedded SVG assets -->
	<xsl:variable name="logo-svg"><![CDATA[
<svg width="202" height="30" viewBox="0 0 202 30" fill="none" xmlns="http://www.w3.org/2000/svg">
<g clip-path="url(#clip0)">
<path d="M13.3249 0H0V13.2975H13.3249V0Z" fill="#00A89F"/>
<path d="M5.94453 14.7525C5.92217 12.9111 6.44898 11.1047 7.45798 9.56294C8.46697 8.02113 9.91259 6.81356 11.611 6.09374C13.3095 5.37392 15.1841 5.17435 16.9965 5.52042C18.8089 5.86648 20.4772 6.74255 21.7894 8.03724C23.1016 9.33193 23.9984 10.9868 24.3658 12.7914C24.7332 14.596 24.5546 16.4689 23.8526 18.1719C23.1507 19.875 21.9572 21.3313 20.4238 22.3557C18.8904 23.3801 17.0864 23.9263 15.2411 23.925C14.0272 23.9319 12.8238 23.7001 11.6996 23.2429C10.5754 22.7857 9.55246 22.112 8.68919 21.2602C7.82592 20.4085 7.13921 19.3954 6.66828 18.2788C6.19735 17.1622 5.95142 15.9639 5.94453 14.7525" fill="#003D7F"/>
<path d="M13.3327 16.7024H0.0078125V29.9999H13.3327V16.7024Z" fill="#00A89F"/>
<path d="M30.4601 0H17.1353V13.2975H30.4601V0Z" fill="#00A89F"/>
<path d="M44.7772 23.4599H42.7555V7.43244H36.7432V5.56494H50.797V7.43244H44.7847L44.7772 23.4599Z" fill="#808184"/>
<path d="M53.36 23.46H51.3608V10.245H53.36V13.695C53.5965 13.161 53.8968 12.6573 54.2543 12.195C54.6057 11.7428 55.0209 11.3436 55.4868 11.01C55.9428 10.6787 56.4507 10.4253 56.9899 10.26C57.5616 10.085 58.1585 10.0064 58.756 10.0275V12.15H58.6208C57.9227 12.1439 57.2303 12.2765 56.5841 12.54C55.932 12.7829 55.3436 13.1701 54.863 13.6725C54.3641 14.2264 53.9809 14.8741 53.7357 15.5775C53.4615 16.4222 53.3344 17.3075 53.36 18.195V23.46Z" fill="#808184"/>
<path d="M63.1297 12.0752C62.4842 12.2555 61.8556 12.4914 61.2509 12.7802L60.6572 11.1677C61.382 10.8307 62.1368 10.562 62.9118 10.3652C63.7566 10.1524 64.6259 10.0515 65.4971 10.0652C67.0103 9.96655 68.5044 10.4466 69.6757 11.4077C70.1865 11.9297 70.5806 12.5539 70.8319 13.2391C71.0832 13.9243 71.1861 14.6549 71.1337 15.3827V23.4602H69.1947V21.4652C68.6792 22.1075 68.0468 22.6469 67.3309 23.0552C66.434 23.5444 65.4213 23.7828 64.3998 23.7452C63.7897 23.7435 63.1827 23.6577 62.5961 23.4902C62.0277 23.3346 61.491 23.0807 61.0104 22.7402C60.5445 22.3977 60.1597 21.9573 59.8831 21.4502C59.5869 20.8917 59.4394 20.2668 59.4547 19.6352C59.4429 18.9936 59.5846 18.3586 59.868 17.7827C60.1371 17.2446 60.5377 16.7828 61.0329 16.4402C61.578 16.0629 62.1863 15.7861 62.8291 15.6227C63.5771 15.4263 64.348 15.3305 65.1213 15.3377C65.8756 15.3299 66.6294 15.3801 67.376 15.4877C67.9996 15.5898 68.6169 15.7275 69.2248 15.9002V15.4427C69.2561 14.9432 69.1767 14.4429 68.9922 13.9776C68.8078 13.5122 68.5228 13.0931 68.1576 12.7502C67.333 12.1009 66.2974 11.7778 65.2491 11.8427C64.5353 11.8239 63.8224 11.9021 63.1297 12.0752V12.0752ZM62.4984 17.5052C62.1792 17.73 61.9199 18.0293 61.7431 18.377C61.5663 18.7247 61.4773 19.1103 61.4839 19.5002C61.4763 19.8912 61.5667 20.2779 61.7469 20.6252C61.9352 20.9529 62.1914 21.2367 62.4984 21.4577C62.8174 21.6944 63.1763 21.8722 63.5581 21.9827C63.9678 22.1043 64.3933 22.165 64.8207 22.1627C65.4078 22.1668 65.9916 22.0756 66.5493 21.8927C67.0552 21.7254 67.5282 21.4716 67.9471 21.1427C68.3414 20.8412 68.6646 20.4572 68.8941 20.0177C69.1281 19.574 69.2469 19.0789 69.2398 18.5777V17.3252C68.7438 17.1902 68.1801 17.0627 67.5413 16.9427C66.8219 16.8159 66.0923 16.7556 65.3618 16.7627C64.3447 16.6921 63.332 16.9526 62.4759 17.5052H62.4984Z" fill="#808184"/>
<path d="M85.278 23.46V20.805C84.9984 21.1984 84.6947 21.5741 84.3686 21.93C84.0331 22.2842 83.6602 22.601 83.2563 22.875C82.8311 23.1527 82.3679 23.3674 81.881 23.5125C81.3367 23.671 80.7721 23.7494 80.205 23.745C78.5727 23.7377 77.0091 23.0882 75.8536 21.9375C75.2287 21.3229 74.7352 20.588 74.4031 19.7775C74.0283 18.8538 73.8443 17.864 73.862 16.8675C73.8459 15.8688 74.0299 14.8769 74.4031 13.95C74.7346 13.1428 75.2224 12.4088 75.8386 11.79C76.4192 11.2145 77.1057 10.7562 77.8602 10.44C79.1349 9.91039 80.5508 9.82323 81.881 10.1925C82.8285 10.4693 83.6881 10.986 84.3761 11.6925C84.7074 12.0178 85.0092 12.3717 85.278 12.75V4.80005H87.247V23.46H85.278ZM84.9247 14.7525C84.6766 14.154 84.3112 13.6109 83.85 13.155C83.4165 12.7214 82.9071 12.3706 82.3469 12.12C81.7838 11.8801 81.1781 11.7551 80.5658 11.7525C79.9429 11.7497 79.3254 11.8668 78.7471 12.0975C78.1819 12.3185 77.6698 12.6559 77.244 13.0875C76.8062 13.5502 76.4663 14.096 76.2444 14.6925C75.9857 15.3804 75.8582 16.1104 75.8686 16.845C75.8594 17.5649 75.9869 18.28 76.2444 18.9525C76.4743 19.5491 76.8161 20.0964 77.2515 20.565C77.6751 21.0104 78.187 21.3629 78.7546 21.6C79.3241 21.8347 79.9346 21.9545 80.5508 21.9525C81.1635 21.9538 81.7699 21.8286 82.3319 21.585C82.8903 21.3346 83.3993 20.9866 83.835 20.5575C84.3052 20.0974 84.6761 19.5461 84.9247 18.9375C85.4557 17.5929 85.4557 16.0972 84.9247 14.7525V14.7525Z" fill="#808184"/>
<path d="M103.473 19.4999C103.144 20.3371 102.654 21.1017 102.03 21.7499C101.403 22.3907 100.654 22.9006 99.8277 23.2499C98.9531 23.6194 98.0118 23.8057 97.062 23.7974C96.1194 23.8076 95.1851 23.6211 94.3189 23.2499C93.5036 22.8925 92.7639 22.3835 92.1394 21.7499C91.5232 21.1225 91.0356 20.3812 90.704 19.5674C90.3625 18.7213 90.1889 17.817 90.1929 16.9049C90.1902 15.9904 90.3637 15.084 90.704 14.2349C91.424 12.5284 92.793 11.1767 94.5105 10.4764C96.2281 9.77619 98.1539 9.78462 99.8653 10.4999C100.684 10.8553 101.426 11.3645 102.052 11.9999C102.671 12.6249 103.159 13.3668 103.488 14.1824C104.179 15.8929 104.179 17.8044 103.488 19.5149L103.473 19.4999ZM101.609 14.8499C101.139 13.6263 100.205 12.6372 99.0085 12.0974C98.3968 11.8223 97.733 11.6816 97.062 11.6849C96.3826 11.6781 95.7093 11.8135 95.0855 12.0824C94.5136 12.3391 94.0017 12.7121 93.5824 13.1774C93.1601 13.6601 92.8268 14.2137 92.5979 14.8124C92.3567 15.4526 92.2369 16.1316 92.2447 16.8149C92.2383 17.5021 92.3633 18.1843 92.6129 18.8249C92.8387 19.4272 93.1822 19.9787 93.6234 20.4474C94.0647 20.916 94.5949 21.2925 95.1832 21.5549C95.7949 21.83 96.4587 21.9707 97.1297 21.9674C97.8091 21.9742 98.4825 21.8387 99.1062 21.5699C99.6774 21.3153 100.189 20.945 100.609 20.4824C101.044 20.0121 101.384 19.4619 101.609 18.8624C102.095 17.5688 102.095 16.1434 101.609 14.8499V14.8499Z" fill="#808184"/>
<path d="M115.64 21.4276C115.398 21.908 115.058 22.3322 114.64 22.6726C114.192 23.0163 113.682 23.2709 113.137 23.4226C112.535 23.6005 111.909 23.689 111.281 23.6851C110.291 23.6809 109.309 23.5032 108.38 23.1601C107.454 22.8349 106.588 22.3584 105.817 21.7501L106.817 20.3476C107.503 20.8725 108.263 21.2945 109.072 21.6001C109.821 21.8818 110.615 22.0291 111.416 22.0351C112.119 22.0656 112.812 21.8683 113.393 21.4726C113.636 21.2987 113.832 21.0671 113.963 20.7987C114.095 20.5303 114.157 20.2335 114.144 19.9351V19.8826C114.151 19.5804 114.055 19.2847 113.874 19.0426C113.669 18.7836 113.413 18.5692 113.122 18.4126C112.783 18.2261 112.428 18.068 112.063 17.9401C111.657 17.8051 111.244 17.6776 110.815 17.5576C110.387 17.4376 109.786 17.2426 109.267 17.0626C108.773 16.8955 108.301 16.6689 107.862 16.3876C107.449 16.1221 107.098 15.7718 106.832 15.3601C106.558 14.9084 106.42 14.3878 106.434 13.8601V13.8001C106.427 13.2662 106.543 12.7377 106.772 12.2551C106.988 11.79 107.306 11.3797 107.704 11.0551C108.13 10.7157 108.617 10.461 109.139 10.3051C109.722 10.121 110.331 10.0299 110.943 10.0351C111.788 10.04 112.627 10.1741 113.431 10.4326C114.225 10.6883 114.983 11.0463 115.685 11.4976L114.791 12.9976C114.202 12.5941 113.569 12.2594 112.904 12.0001C112.265 11.7749 111.591 11.6583 110.913 11.6551C110.247 11.6178 109.588 11.8112 109.049 12.2026C108.833 12.3601 108.657 12.5667 108.537 12.8054C108.417 13.0441 108.355 13.308 108.358 13.5751V13.6201C108.352 13.9143 108.451 14.2011 108.636 14.4301C108.845 14.677 109.1 14.8809 109.387 15.0301C109.736 15.2157 110.1 15.3689 110.477 15.4876C110.883 15.6226 111.311 15.7651 111.755 15.9001C112.198 16.0351 112.777 16.2226 113.258 16.4101C113.742 16.588 114.202 16.8275 114.625 17.1226C115.022 17.4016 115.357 17.759 115.61 18.1726C115.87 18.6291 116 19.1478 115.986 19.6726V19.7251C116.01 20.3122 115.891 20.8963 115.64 21.4276V21.4276Z" fill="#808184"/>
<path d="M128.183 11.1749C128.345 11.5469 128.605 11.8682 128.935 12.1049C129.394 12.431 129.901 12.684 130.438 12.8549C131.235 13.1248 132.048 13.3428 132.873 13.5074C134.469 13.7752 135.981 14.4065 137.292 15.3524C137.764 15.748 138.137 16.2469 138.383 16.8102C138.63 17.3735 138.742 17.986 138.713 18.5999C138.728 19.3134 138.582 20.0212 138.286 20.6705C137.989 21.3198 137.549 21.8939 136.999 22.3499C136.43 22.8133 135.777 23.1622 135.075 23.3774C134.278 23.6177 133.45 23.7366 132.618 23.7299C131.276 23.7451 129.943 23.5111 128.687 23.0399C127.434 22.5359 126.283 21.8091 125.29 20.8949L126.545 19.3949C127.38 20.1819 128.34 20.8256 129.386 21.2999C130.432 21.737 131.559 21.9515 132.693 21.9299C133.73 21.981 134.754 21.6825 135.601 21.0824C135.952 20.8232 136.234 20.4831 136.423 20.0912C136.613 19.6994 136.705 19.2674 136.691 18.8324C136.699 18.4421 136.627 18.0543 136.481 17.6924C136.322 17.3354 136.076 17.0235 135.767 16.7849C135.331 16.464 134.85 16.2111 134.339 16.0349C133.58 15.7695 132.804 15.5565 132.016 15.3974C131.104 15.215 130.208 14.9643 129.333 14.6474C128.65 14.4064 128.012 14.0514 127.447 13.5974C126.969 13.2149 126.59 12.7237 126.342 12.1649C126.088 11.5494 125.965 10.8879 125.981 10.2224C125.972 9.54019 126.122 8.8651 126.417 8.24993C126.709 7.64843 127.13 7.11839 127.65 6.69743C128.205 6.24858 128.836 5.90307 129.514 5.67743C130.274 5.42199 131.071 5.29524 131.874 5.30243C133.044 5.2811 134.21 5.46645 135.316 5.84993C136.349 6.23564 137.313 6.7852 138.171 7.47743L136.999 9.03743C136.247 8.401 135.393 7.89406 134.474 7.53743C133.623 7.23056 132.725 7.07569 131.821 7.07993C131.288 7.07156 130.758 7.1501 130.25 7.31243C129.813 7.44379 129.404 7.65207 129.04 7.92743C128.723 8.17429 128.466 8.48969 128.289 8.84993C128.108 9.20531 128.015 9.59884 128.018 9.99743C127.994 10.397 128.05 10.7974 128.183 11.1749V11.1749Z" fill="#808184"/>
<path d="M144.371 19.7024C144.337 20.0159 144.374 20.3331 144.479 20.6304C144.585 20.9278 144.756 21.1976 144.98 21.4199C145.448 21.7588 146.018 21.9281 146.596 21.8999C146.919 21.9032 147.242 21.8705 147.558 21.8024C147.892 21.7216 148.215 21.6007 148.52 21.4424V23.1299C148.177 23.318 147.811 23.4592 147.43 23.5499C146.512 23.7585 145.557 23.7379 144.65 23.4899C144.212 23.3595 143.807 23.137 143.462 22.8374C143.124 22.5136 142.867 22.1155 142.711 21.6749C142.509 21.1251 142.412 20.5426 142.425 19.9574V11.9999H140.539V10.2449H142.38V6.25488H144.356V10.2449H148.558V11.9999H144.371V19.7024Z" fill="#808184"/>
<path d="M160.83 10.2451H162.777V23.4601H160.83V21.1576C160.381 21.8802 159.787 22.5031 159.087 22.9876C158.255 23.5209 157.278 23.783 156.291 23.7376C155.562 23.7501 154.836 23.6202 154.157 23.3551C153.555 23.1116 153.012 22.7435 152.564 22.2751C152.121 21.7885 151.784 21.2168 151.571 20.5951C151.332 19.9056 151.215 19.1797 151.226 18.4501V10.2451H153.195V18.0001C153.136 19.0563 153.473 20.0965 154.142 20.9176C154.488 21.2838 154.91 21.5697 155.379 21.755C155.848 21.9403 156.351 22.0206 156.855 21.9901C157.395 21.9946 157.931 21.8953 158.433 21.6976C158.908 21.5088 159.336 21.2197 159.688 20.8501C160.047 20.4581 160.326 19.9994 160.507 19.5001C160.718 18.9625 160.822 18.3896 160.815 17.8126L160.83 10.2451Z" fill="#808184"/>
<path d="M177.177 23.46V20.805C176.902 21.202 176.598 21.578 176.267 21.93C175.934 22.2861 175.56 22.6032 175.155 22.875C174.733 23.1543 174.272 23.3692 173.787 23.5125C173.243 23.6705 172.678 23.7488 172.111 23.745C171.311 23.7452 170.52 23.5872 169.781 23.28C169.023 22.9704 168.333 22.5142 167.752 21.9375C167.135 21.3196 166.647 20.5853 166.317 19.7775C165.948 18.8522 165.767 17.8632 165.783 16.8675C165.768 15.8695 165.949 14.8782 166.317 13.95C166.651 13.1443 167.139 12.4108 167.752 11.79C168.337 11.216 169.026 10.758 169.781 10.44C170.518 10.127 171.311 9.96627 172.111 9.96755C172.683 9.96247 173.252 10.0382 173.802 10.1925C174.288 10.3379 174.752 10.5446 175.185 10.8075C175.59 11.0607 175.963 11.3602 176.297 11.7C176.619 12.0256 176.913 12.3767 177.177 12.75V4.80005H179.153V23.46H177.177ZM176.861 14.7525C176.613 14.154 176.247 13.6109 175.786 13.155C175.341 12.7181 174.819 12.3672 174.246 12.12C173.683 11.8793 173.077 11.7543 172.464 11.7525C171.842 11.7503 171.224 11.8674 170.646 12.0975C170.082 12.3203 169.57 12.6574 169.143 13.0875C168.707 13.5521 168.368 14.0974 168.143 14.6925C167.891 15.3818 167.766 16.1112 167.775 16.845C167.767 17.5641 167.892 18.2786 168.143 18.9525C168.373 19.5509 168.717 20.0987 169.158 20.565C169.581 21.0104 170.093 21.3629 170.661 21.6C171.228 21.8347 171.836 21.9545 172.449 21.9525C173.062 21.9544 173.669 21.8293 174.231 21.585C174.802 21.3379 175.324 20.9898 175.771 20.5575C176.236 20.096 176.602 19.5448 176.846 18.9375C177.377 17.5929 177.377 16.0972 176.846 14.7525H176.861Z" fill="#808184"/>
<path d="M182.971 7.35762V5.18262H185.225V7.35762H182.971ZM183.098 23.4601V10.2451H185.068V23.4601H183.098Z" fill="#808184"/>
<path d="M201.474 19.4999C201.142 20.3356 200.653 21.0997 200.031 21.7499C199.403 22.393 198.651 22.9031 197.821 23.2499C196.947 23.6208 196.005 23.8073 195.056 23.7974C194.115 23.8081 193.183 23.6216 192.32 23.2499C191.504 22.8943 190.764 22.385 190.14 21.7499C189.524 21.1225 189.037 20.3812 188.705 19.5674C188.363 18.7213 188.19 17.817 188.194 16.9049C188.191 15.9904 188.365 15.084 188.705 14.2349C189.425 12.5284 190.794 11.1767 192.512 10.4764C194.229 9.77619 196.155 9.78462 197.866 10.4999C198.685 10.8553 199.427 11.3645 200.053 11.9999C200.67 12.6272 201.157 13.3686 201.489 14.1824C201.83 15.0285 202.004 15.9327 202 16.8449C202.003 17.756 201.824 18.6585 201.474 19.4999V19.4999ZM199.602 14.8499C199.367 14.2401 199.015 13.6822 198.565 13.2074C198.124 12.7367 197.592 12.3592 197.002 12.0974C196.391 11.8209 195.727 11.6802 195.056 11.6849C194.378 11.678 193.708 11.8135 193.086 12.0824C192.513 12.3366 192.001 12.7099 191.583 13.1774C191.158 13.6578 190.824 14.212 190.599 14.8124C190.355 15.4518 190.233 16.1308 190.238 16.8149C190.234 17.5026 190.362 18.1848 190.614 18.8249C190.834 19.4266 191.171 19.9789 191.605 20.45C192.04 20.9212 192.564 21.3018 193.147 21.5699C193.758 21.845 194.422 21.9856 195.093 21.9824C195.773 21.9892 196.446 21.8537 197.07 21.5849C197.64 21.329 198.152 20.9589 198.573 20.4974C199.008 20.0271 199.347 19.4769 199.572 18.8774C200.063 17.5847 200.063 16.1575 199.572 14.8649L199.602 14.8499Z" fill="#808184"/>
</g>
<defs>
<clipPath id="clip0">
<rect width="202" height="30" fill="white"/>
</clipPath>
</defs>
</svg>
]]>
	</xsl:variable>
	<!--<xsl:variable name="logo-data" select="concat('data:image/svg+xml;utf8,', $logo-svg)"/>-->

	<xsl:variable name="error-svg"><![CDATA[
<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16">
  <circle cx="8" cy="8" r="7" fill="#d32f2f" stroke="#a00000" stroke-width="0.5"/>
  <rect x="7" y="3.5" width="2" height="6.5" fill="#ffffff"/>
  <rect x="7" y="11.5" width="2" height="2.5" fill="#ffffff"/>
</svg>
]]>
	</xsl:variable>
	<!--<xsl:variable name="error-data" select="concat('data:image/svg+xml;utf8,', $error-svg)"/>-->

	<xsl:variable name="warning-svg"><![CDATA[
<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16">
  <polygon points="8,2 14,14 2,14" fill="#f57c00" stroke="#b26000" stroke-width="0.5"/>
  <rect x="7" y="4" width="2" height="6" fill="#ffffff"/>
  <rect x="7" y="11" width="2" height="2" fill="#ffffff"/>
</svg>
]]>
	</xsl:variable>
	<!--<xsl:variable name="warning-data" select="concat('data:image/svg+xml;utf8,', $warning-svg)"/>-->

	<xsl:template match="/task">
		<html>
			<head>
				<style type="text/css">
body {
    color: #636463;
    font-family: Segoe UI;
    font-size: 13px;
    line-height: 20px;
}

.inline-icon {
    display: inline-block;
    vertical-align: middle;
    margin-right: 4px;
}

.inline-icon.logo {
    margin-right: 0;
}

.InfoTable {
    border: none;
    border-collapse: collapse;
    font-family: Segoe UI;
    font-size: 10pt;
}

.InfoHeader {
    border-bottom: #e5ecf0 1px solid;
    border-right: #e5ecf0 1px solid;
    color: #4086AA;
    background-color: #f8fafa;
    font-family: Segoe UI;
    font-size: 10pt;
}

.InfoItem {
    font-weight: bold;
    color: #0A1E2C;
    font-family: Segoe UI;
    font-size: 13px;
    padding-right: 40px;
    white-space: nowrap !important;
    line-height: 20px;
    width: 30%;
    overflow: hidden;
}

.InfoItemWithIndent {
    font-weight: bold;
    color: #0A1E2C;
    font-family: Segoe UI;
    font-size: 13px;
    padding-left: 30px;
    padding-right: 40px;
    white-space: nowrap;
    line-height: 20px;
}

.InfoData {
    color: #636463;
    font-family: Segoe UI;
    font-size: 13px;
    line-height: 20px;
}

.InfoDataMinColWidth {
    width: 10%;
}

.ConversionReportTable {
    border: #999999 1px solid;
    border-collapse: collapse;
    color: #4086AA;
    font-family: Segoe UI;
    font-size: 10pt;
}

    .ConversionReportTable td {
        color: #4086AA;
        text-align: left;
    }

.ReportTable {
    border: #BFCDD4 1px solid;
    border-collapse: collapse;
    color: #636463;
    font-family: Segoe UI;
    font-size: 13px;
    line-height: 20px;
}

    .ReportTable th {
        text-align: left;
        background-color: #F8FAFA;
        font-size: 13px;
        line-height: 20px;
        padding: 5px;
        color: #0A1E2C;
    }

        .ReportTable th.Unit {
            text-align: right;
        }

        .ReportTable th.UnitWrap {
            white-space: normal;
        }

    .ReportTable tr {
        text-align: left;
        background-color: #ffffff;
        border-bottom: #E5ECF0 1px solid;
    }

        .ReportTable tr:nth-child(even) {
            background: #F8FAFA;
        }

        .ReportTable tr:nth-child(odd) {
            background: #fff;
        }

    .ReportTable td {
        text-align: right;
        padding: 5px;
    }

        .ReportTable td.File {
            color: #0A1E2C;
            text-align: left;
        }

    .ReportTable tr {
        vertical-align: top;
    }

    .ReportTable td.MergedFile {
        border-top: #e5ecf0 1px solid;
        border-right: #e5ecf0 1px solid;
        color: #0A1E2C;
        text-align: left;
        font-weight: normal;
        vertical-align: top;
        white-space: normal;
        word-break: break-all;
        padding-left: 10%;
        background-color: White;
    }

    .ReportTable td.Text {
        color: #4086AA;
        text-align: left;
        font-size: 8pt;
    }

    .ReportTable td.TextIndented {
        color: #4086AA;
        text-align: left;
        padding-left: 20px;
        font-size: 8pt;
    }

.TypeHead {
    white-space: nowrap;
    text-align: left;
    background-color: #f8fafa;
    border-bottom: #E5ECF0 1px solid;
}

.File {
    border-top: #E5ECF0 1px solid;
    border-right: #E5ECF0 1px solid;
    color: #0A1E2C;
    text-align: left;
    background-color: #f8fafa;
    font-weight: normal;
    word-break: break-all;
}

.Number {
    text-align: right;
}

.CharPerWord {
    border-bottom: #e5ecf0 1px solid;
    border-right: #e5ecf0 1px solid;
    color: #0A1E2C;
    font: normal italic 10pt;
    white-space: nowrap;
    text-align: left;
    background-color: #f8fafa;
}

.Type {
    border-left: #e5ecf0 1px solid;
}

.Unit {
    border-bottom: #e5ecf0 1px solid;
    padding-left: 4px;
    color: #4086AA;
    white-space: nowrap;
    text-align: center;
    background-color: #f8fafa;
    border-bottom: #e5ecf0 1px solid;
}

.UnitWrap {
}

.Header {
    white-space: nowrap;
    text-align: left;
    background-color: #f8fafa;
}

.HeaderWithIndent {
    white-space: nowrap;
    text-align: left;
    background-color: #f8fafa;
    padding-left: 0.5cm;
}

.HeaderWithLineAbove {
    white-space: nowrap;
    text-align: left;
    background-color: #f8fafa;
    border-top: #e5ecf0 1px solid;
}

.WithLineAbove {
    white-space: nowrap;
    border-top: #e5ecf0 1px solid;
}




.Total {
    font-weight: bold;
    color: #0A1E2C !important;
    white-space: nowrap;
    text-align: left;
    background-color: #F0F4F6 !important;
    border-top: #bfcdd4 1px solid !important;
    border-bottom: #bfcdd4 1px solid !important;
}

.Error {
    color: red;
}

.MergedFile {
    border-top: #e5ecf0 1px solid;
    border-right: #e5ecf0 1px solid;
    color: #0A1E2C;
    text-align: left;
    font-weight: normal;
    vertical-align: top;
    white-space: normal;
    word-break: break-all;
    padding-left: 10%;
    background-color: White;
}

.MergedCharPerWord {
    border-bottom: #e5ecf0 1px solid;
    border-right: #e5ecf0 1px solid;
    color: #0A1E2C;
    font: normal italic 10pt;
    white-space: nowrap;
    text-align: left;
    padding-left: 10%;
    background-color: White;
}

.MergedHeader {
    white-space: nowrap;
    text-align: left;
    background-color: White;
}

.MergedHeaderWithIndent {
    white-space: nowrap;
    text-align: left;
    background-color: White;
    padding-left: 0.5cm;
}

.MergedHeaderWithLineAbove {
    white-space: nowrap;
    text-align: left;
    background-color: White;
    border-top: #e5ecf0 1px solid;
}

.MergedTotal {
    border-top: #e5ecf0 1px solid;
    border-bottom: #e5ecf0 1px solid;
    font-weight: bold;
    white-space: nowrap;
    text-align: left;
    background-color: White;
}

/*
	Verification Reports
*/
.TitleTable {
    border: #999999 1px solid;
    border-collapse: collapse;
    font-size: 14pt;
    color: #333399;
    background-color: #e2e2e2;
}


    .TitleTable th {
        text-align: left;
        white-space: nowrap;
    }

.VerificationReportTable {
    border: #999999 1px solid;
    border-collapse: collapse;
    color: #0000cc;
    font-family: Segoe UI;
    font-size: 10pt;
}

    .VerificationReportTable th {
        white-space: nowrap;
        text-align: left;
        background-color: #f8fafa;
        border-bottom: #e5ecf0 1px solid;
        border-top: #e5ecf0 1px solid;
        color: #4086AA;
    }

    .VerificationReportTable td {
        border-bottom: #e5ecf0 1px solid;
        border-top: #e5ecf0 1px solid;
        color: #0A1E2C;
    }

    .VerificationReportTable a {
        color: #4086AA;
    }

.VerificationStatisticsTable {
    border: #999999 1px solid;
    border-collapse: collapse;
    font-family: Segoe UI;
    font-size: 10pt;
}

    .VerificationStatisticsTable td {
        border-bottom: #e5ecf0 1px solid;
        border-top: #e5ecf0 1px solid;
        border-right: #e5ecf0 1px solid;
        margin-left: 2px;
    }


h1 {
    font-family: Segoe UI;
    font-weight: normal;
    font-size: 22px;
    color: #636463;
    padding: 0;
    margin: 0px;
}

h2.first {
    font-family: Segoe UI;
    line-height: 22px;
    font-size: 16px;
    color: #0A1E2C;
    padding: 20px 0 2px 0;
    margin: 0px 0 2px 0;
}

h2 {
    font-family: Segoe UI;
    line-height: 22px;
    font-size: 16px;
    color: #0A1E2C;
    padding: 20px 0 2px 0;
    margin: 0px 0 2px 0;
}

.InfoList {
    font-family: Segoe UI;
    font-size: 10pt;
    padding-left: 0px;
    margin-left: 0px;
    padding-top: 0px;
    margin-top: 0px;
    table-layout: auto;
    border-collapse: collapse;
    width: 100%;
}

.InfoListItem {
    font-family: Segoe UI;
    font-size: 10pt;
    padding-left: 0px;
    margin-left: 0px;
}


.InfoMessage {
    font-family: Segoe UI;
    font-size: 10pt;
    padding-top: 0px;
    padding-left: 0px;
    margin-left: 0px;
    margin-top: 10px;
    margin-bottom: 10px;
}
          
					<!-- Add styles for the QA Providers tree view -->
					.qa-providers-container .enabled.disabled { color: gray !important; }
					.qa-providers-container { margin: 20px 0; }
					.qa-providers-container ul { list-style: none; margin: 0; padding: 0; }
					.qa-providers-container li { margin: 4px 0; }
					.qa-providers-container .toggle {
					cursor: pointer;
					font-weight: bold;
					display: inline-block;
					width: 12px;
					text-align: center;
					user-select: none;
					}
					.qa-providers-container .invisible { visibility: hidden; }
					.qa-providers-container .children { display: none; margin-left: 20px; margin-top: 4px; }
					.qa-providers-container li.expanded > .children { display: block; }
					.qa-providers-container .value { color: red; }
					.qa-providers-container .enabled {color: green; }
				</style>
        <script>
          <![CDATA[
					
					function OpenFileAndRunVerification(guid)
					{
						if (window.external)
						{
							try
							{
							    window.chrome.webview.postMessage(guid);
							}
							catch (e)
							{
								document.getElementById(guid).click();
							}
						}
						else
						{
							document.getElementById(guid).click();
						}
					}
					
					function OpenFileAndGoToSegment(guid, paragraph, segment)
					{
						if (window.external)
						{
							try
							{
							    window.chrome.webview.postMessage(guid + "/" + paragraph + "/" + segment);
							}
							catch (e)
							{
								document.getElementById(guid).click();
							}
						}
						else
						{
							document.getElementById(guid).click();
						}
					}
					
					function toggleCategory(toggle) {
						console.log("toggle");
						var li = toggle.parentElement;
						
						if (li.classList.contains('expanded')) {
							li.classList.remove('expanded');
							toggle.textContent = '+';
						} else {
							li.classList.add('expanded');
							toggle.textContent = '–';
						}
						
						
						
					}
					
				window.addEventListener('DOMContentLoaded', function() {
					// Set logo image
					var others = document.querySelectorAll('.other-img');
					others.forEach(function(img) {
						img.src = getLogoUrl(img.getAttribute('data-img'));
					});
					var images = document.querySelectorAll('.flag-img');
					images.forEach(function(img) {
						var lcid = img.getAttribute('data-lcid');
						// Construct the flag URL as needed
						img.src = getFlagUrl(lcid);
					});
				});

				function getFlagUrl(lcid) {
					// Adjust the path as needed for your deployment
					return 'C:/Program Files (x86)/Trados/Trados Studio/Studio18Beta/ReportResources/images/Flags/' + lcid + '.bmp';
				}
				
				function getLogoUrl(imgName) {
				// Adjust the path as needed for your deployment
					return 'C:/Program Files (x86)/Trados/Trados Studio/Studio18Beta/ReportResources/images/' + imgName;
				}

           ]]>
        </script>
			</head>
			<body>

				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td width="100%">
							<h1>
								Verify Files Report
							</h1>
						</td>
						<td valign="top">
              <div class="inline-icon logo">
                <xsl:value-of select="$logo-svg" disable-output-escaping="yes"/>
              </div>
						</td>
					</tr>
				</table>

				<h2 class="first">
					Summary
				</h2>

				<table class="InfoTable" border="0" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<td class="InfoItem">
							Project:
						</td>
						<td class="InfoData">
							<xsl:value-of select="//taskInfo/project/@name"/>
						</td>
					</tr>

					<xsl:if test="//customer/@name">
						<tr>
							<td class="InfoItem">
								VerificationReport_Customer
							</td>
							<td class="InfoData">
								<xsl:value-of select="//taskInfo/customer/@name"/>
								<xsl:if test="//customer/@email">
									&#32;(<xsl:value-of select="//taskInfo/customer/@email"/>)
								</xsl:if>
							</td>
						</tr>
					</xsl:if>

					<xsl:if test="//project/@dueDate">
						<tr>
							<td class="InfoItem">
								VerificationReport_DueDate
							</td>
							<td class="InfoData">
								<xsl:value-of select="//taskInfo/project/@dueDate"/>
							</td>
						</tr>
					</xsl:if>

					<tr>
						<td class="InfoItem">
							Files:
						</td>
						<td class="InfoData">
							<xsl:value-of select="count(//task/file)"/>
							<xsl:if test="//taskInfo/project/@projectFilesTotal">
								<xsl:text> / </xsl:text>
								<xsl:value-of select="//taskInfo/project/@projectFilesTotal"/>
							</xsl:if>
						</td>
					</tr>

					<tr>
						<td class="InfoItem">
							Created At:
						</td>
						<td class="InfoData">
							<xsl:value-of select="//taskInfo/@runAt"/>
						</td>
					</tr>
				</table>

				<!-- Add QA Providers section if it exists -->
				<xsl:if test="VerificationSettings">
					<h2>
						Active QA Providers
					</h2>
					<div class="qa-providers-container">
						<xsl:apply-templates select="VerificationSettings" mode="qa-tree"/>
					</div>
				</xsl:if>

				<h2>
					Statistics
				</h2>

				<table border="0" cellspacing="10" cellpadding="0">
					<tr>
						<td valign="top">
							<xsl:call-template name="overview" />
						</td>
					</tr>
				</table>

				<h2>
					File Details
				</h2>

				<table border="0" cellspacing="10" cellpadding="0" width="100%" >
					<tr>
						<td colspan="2">
							<xsl:apply-templates select="file" />
						</td>
					</tr>
				</table>
			</body>
		</html>
	</xsl:template>

	<!-- Non-leaf node template -->
	<xsl:template match="*[*]" mode="qa-tree">
		<xsl:param name="isDisabled" select="false()" />
		<ul>
			<li>
				<span class="toggle" onclick="toggleCategory(this)">+</span>
				<xsl:text> </xsl:text>
				<xsl:value-of select="@Name"/>
				<xsl:if test="@Value">
					<xsl:if test="@Name and string-length(@Name) > 0 and @Value and string-length(@Value) > 0">
						:
					</xsl:if>
					<span>
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="$isDisabled">
									<xsl:text>value enabled disabled</xsl:text>
								</xsl:when>
								<xsl:when test="@Value = 'False'">
									<xsl:text>value</xsl:text>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text>enabled</xsl:text>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:value-of select="@Value"/>
					</span>
				</xsl:if>
				<ul class="children">
					<xsl:apply-templates select="*" mode="qa-tree">
						<xsl:with-param name="isDisabled" select="$isDisabled or @Value = 'False'" />
					</xsl:apply-templates>
				</ul>
			</li>
		</ul>
	</xsl:template>

	<!-- Leaf node template -->
	<xsl:template match="*[not(*)]" mode="qa-tree">
		<xsl:param name="isDisabled" select="false()" />
		<li>
			<span class="toggle invisible">&#160;</span>
			<xsl:text> </xsl:text>
			<xsl:value-of select="@Name"/>
			<xsl:if test="@Value">
				<xsl:if test="@Name and string-length(@Name) > 0 and @Value and string-length(@Value) > 0">
					:
				</xsl:if>
				<span>
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$isDisabled">
								<xsl:text>value enabled disabled</xsl:text>
							</xsl:when>
							<xsl:when test="@Value = 'False'">
								<xsl:text>value</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>enabled</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<xsl:value-of select="@Value"/>
				</span>
			</xsl:if>
		</li>
	</xsl:template>

	<xsl:template match="plugin">
		<tr>
			<td class="InfoItem">
				<xsl:choose>
					<xsl:when test="position() = 1">
						VerificationReport_ActiveVerifiers
					</xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="InfoData">
				<xsl:value-of select="@name" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template name="overview">
		<xsl:variable name="nErrors" select="count(file/Messages/Message[(ErrorLevel='Error' or ErrorLevel='Unspecified') and Ignored='false'])" />
		<xsl:variable name="nWarnings" select="count(file/Messages/Message[ErrorLevel='Warning' and Ignored='false'])" />
		<xsl:variable name="nInformation" select="count(file/Messages/Message[ErrorLevel='Note' and Ignored='false'])" />
		<xsl:variable name="nIgnored" select="count(file/Messages/Message[Ignored='true'])" />

		<table class="VerificationStatisticsTable" border="0" cellspacing="0" cellpadding="2" width="200px">
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nErrors > 0">font-weight:bold;color:red;</xsl:if>
				</xsl:attribute>
				<td>
					Errors:
				</td>
				<td>
					<xsl:value-of select="$nErrors"/>
				</td>
			</tr>
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nWarnings > 0">font-weight:bold;</xsl:if>
				</xsl:attribute>
				<td>
					Warnings:
				</td>
				<td>
					<xsl:value-of select="$nWarnings"/>
				</td>
			</tr>
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nInformation > 0">font-weight:bold;</xsl:if>
				</xsl:attribute>
				<td>
					Information:
				</td>
				<td>
					<xsl:value-of select="$nInformation"/>
				</td>
			</tr>
			<tr	style="font-weight:bold;background-color:#aed0f0;">
				<td>
					Total:
				</td>
				<td>
					<xsl:value-of select="$nErrors + $nWarnings + $nInformation"/>
				</td>
			</tr>
			<tr>
				<td>
					<br/>
				</td>
				<td>
					<br/>
				</td>
			</tr>
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nIgnored > 0">font-weight:bold;color:red</xsl:if>
				</xsl:attribute>
				<td>
					Ignored:
				</td>
				<td>
					<xsl:value-of select="$nIgnored"/>
				</td>
			</tr>
		</table>
	</xsl:template>


	<xsl:template match="file">

		<table class="VerificationReportTable" border="0" cellspacing="0" cellpadding="2">

			<tr>
				<th>
					<!--Condition the header colspan for the two different versions-->
					<xsl:choose>
						<xsl:when test="//taskInfo/@version">
							<xsl:attribute name="colspan">
								9
							</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="colspan">
								6
							</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:attribute name="style">
						<xsl:if test="count(Messages/Message[ErrorLevel='Error' or ErrorLevel='Unspecified'])">color:red;</xsl:if>
					</xsl:attribute>
					&#160;
					<a>
						<xsl:attribute name="href">
							javascript:OpenFileAndRunVerification('<xsl:value-of select="@guid"/>')
						</xsl:attribute>
						<xsl:value-of select="@name"/>
					</a>
					<a>
						<xsl:attribute name="id">
							<xsl:value-of select="@guid"/>
						</xsl:attribute>
						<xsl:attribute name="style">display:none</xsl:attribute>
						<xsl:attribute name="href">
							<xsl:value-of select="@path"/>
						</xsl:attribute>
						<xsl:value-of select="@name"/>
					</a>
				</th>
			</tr>

			<xsl:choose>
				<xsl:when test="(Messages/@includeIgnored='False' and count(Messages/Message[Ignored='false'])=0) or (Messages/@includeIgnored='True' and count(Messages/Message)=0)">
					<tr>
						<td valign="top">&#160;</td>
						<td valign="top"></td>
						<td valign="top" colspan="3" width="100%">
							VerificationReport_NoErrorsReported
						</td>
						<td valign="top">&#160;</td>
					</tr>
				</xsl:when>

				<xsl:otherwise>
					<tr style="color:gray">
						<td valign="top">&#160;</td>
						<td align="center" valign="top" style="font-weight:bold;color:gray">
							&#160;Segment&#160;
						</td>
						<td valign="top">&#160;</td>
						<td valign="top" style="font-weight:bold;color:gray">
							Type
						</td>
						<td valign="top" style="font-weight:bold;color:gray">
							Message
						</td>

						<xsl:if test="//taskInfo/@version">
							<td id="newSourceSegment" valign="top" style="font-weight:bold;color:gray">
								Source
							</td>
							<td id="newTargetSegment" valign="top" style="font-weight:bold;color:gray">
								Target
							</td>
						</xsl:if>

						<td valign="top" style="font-weight:bold;color:gray">
							Verifier
						</td>
					</tr>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</table>

		<!-- Add a gap between each file -->
		&#160;&#160;&#160;


	</xsl:template>

	<xsl:template match="Message">

		<xsl:variable name="IncludeIgnored" select="../@includeIgnored"/>

		<xsl:if test="Ignored='false' or $IncludeIgnored ='True'">

			<xsl:variable name="GrayStyle">
				<xsl:if test="Ignored='true'">
					color:gray;
				</xsl:if>
			</xsl:variable>

			<tr>
				<xsl:attribute name="data-status">
					<xsl:value-of select="Status"/>
				</xsl:attribute>
				<td valign="top">&#160;</td>

				<td valign="top" align="left" style="{$GrayStyle}">
					<a>
						<xsl:attribute name="href">
							javascript:OpenFileAndGoToSegment('<xsl:value-of select="../../@guid"/>', '<xsl:value-of select="ParagraphUnitId"/>', '<xsl:value-of select="SegmentId"/>')
						</xsl:attribute>
						<xsl:value-of select="SegmentId"/>
					</a>
				</td>

				<xsl:apply-templates select="ErrorLevel"/>

				<!-- condition also the view of the columns-->
				<td>
					<xsl:choose>
						<xsl:when test="//taskInfo/@version">
							<xsl:attribute name="width">
								40%
							</xsl:attribute>
							<xsl:attribute name ="valign">
								top
							</xsl:attribute>
							<xsl:attribute name ="align">
								left
							</xsl:attribute>
							<xsl:attribute name ="style">
								{$GrayStyle}
							</xsl:attribute>
						</xsl:when>

						<xsl:otherwise>
							<xsl:attribute name="width">
								100%
							</xsl:attribute>
							<xsl:attribute name ="valign">
								top
							</xsl:attribute>
							<xsl:attribute name ="align">
								left
							</xsl:attribute>
							<xsl:attribute name ="style">
								{$GrayStyle}
							</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:value-of select="Text"/>
				</td>

				<xsl:if test="//taskInfo/@version">
					<td valign="top" align="left" width ="30%" style="white-space:normal; {$GrayStyle}">
						<xsl:value-of select="Source"/>
					</td>
					<td valign="top" align ="left" width ="30%" style="white-space:normal; {$GrayStyle}">
						<xsl:value-of select = "Target" />
					</td>
				</xsl:if>

				<td valign="top" style="white-space:nowrap; {$GrayStyle}">
					<xsl:value-of select="Origin"/>
				</td>
			</tr>
		</xsl:if>

	</xsl:template>

	<xsl:template match="ErrorLevel">
		<td valign="top">
			<xsl:choose>
				<xsl:when test="text()='Error'">
          <div class="inline-icon">
            <xsl:value-of select="$error-svg" disable-output-escaping="yes"/>
          </div>
				</xsl:when>
				<xsl:when test="text()='Warning'">
          <div class="inline-icon">
            <xsl:value-of select="$warning-svg" disable-output-escaping="yes"/>
          </div>
				</xsl:when>
				<xsl:when test="text()='Note'">&#160;</xsl:when>
				<xsl:when test="text()='Unspecified'">
          <div class="inline-icon">
            <xsl:value-of select="$error-svg" disable-output-escaping="yes"/>
          </div>				
				</xsl:when>
			</xsl:choose>
		</td>

		<xsl:variable name="GrayStyle">
			<xsl:if test="../Ignored='true'">
				color:gray;
			</xsl:if>
		</xsl:variable>

		<td valign="top" style="white-space: nowrap;padding-right:10px; {$GrayStyle}">
			<xsl:choose>
				<xsl:when test="text()='Error'">
					Error
				</xsl:when>
				<xsl:when test="text()='Warning'">
					Warning
				</xsl:when>
				<xsl:when test="text()='Note'">
					Information
				</xsl:when>
				<xsl:when test="text()='Unspecified'">
					UnknownError
				</xsl:when>
			</xsl:choose>
		</td>
	</xsl:template>
</xsl:stylesheet>