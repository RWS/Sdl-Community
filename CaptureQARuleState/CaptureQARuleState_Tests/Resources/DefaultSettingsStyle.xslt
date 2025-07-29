<?xml version="1.0" ?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    version="1.0">

	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<style>
					body { font-family: Arial, sans-serif; margin: 20px; }
					ul { list-style: none; margin: 0; padding: 0; }
					li { margin: 4px 0; }
					.toggle {
					cursor: pointer;
					font-weight: bold;
					display: inline-block;
					width: 12px;
					text-align: center;
					user-select: none;
					}
					.invisible { visibility: hidden; }
					.children { display: none; margin-left: 20px; margin-top: 4px; }
					.expanded .children { display: block; }
					.value { color: red; }
				</style>
			</head>
			<body>
				<h1>
					<xsl:value-of select="/*/@Name"/>
				</h1>
				<ul>
					<xsl:apply-templates select="/*/*[*]"/>
				</ul>

				<script>
					function toggleCategory(toggle) {
					var li = toggle.parentElement;
					if (li.classList.contains('expanded')) {
					li.classList.remove('expanded');
					toggle.textContent = '+';
					} else {
					li.classList.add('expanded');
					toggle.textContent = '–';
					}
					}
				</script>
			</body>
		</html>
	</xsl:template>

	<!-- Template for any element that has children (categories) -->
	<xsl:template match="*[*]">
		<li>
			<span class="toggle" onclick="toggleCategory(this)">+</span>
			<xsl:text> </xsl:text>
			<xsl:value-of select="@Name"/>
			<ul class="children">
				<xsl:apply-templates select="*"/>
			</ul>
		</li>
	</xsl:template>

	<!-- Template for any element that has no children (leaf items) -->
	<xsl:template match="*[not(*)]">
		<li>
			<span class="toggle invisible">&#160;</span>
			<xsl:text> </xsl:text>
			<xsl:value-of select="@Name"/>: <span class="value">
				<xsl:value-of select="."/>
			</span>
		</li>
	</xsl:template>
</xsl:stylesheet>