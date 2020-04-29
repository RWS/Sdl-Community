<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/2000/svg">
<xsl:output method="html" indent="yes"/>

<xsl:template match="/">

<html>
	<head>
		<meta http-equiv="X-UA-Compatible" content="IE=9"/>
		<title>Term Report</title>
		
		<!--Developed by Patrick Andrew Hartnett 14/01/2019-->
		
		<style type="text/css">
		  body
		  {
			  background: #FFFFFF;
			  font-family: Verdana, Arial, Helvetica;
			  font-size: 8pt;
		  }
		  a
		  {
			color: #666699;
		  }
		  li
		  {
			margin-top: 3;
		  }
		  h1
		  {
			font-size: 18pt;
		  }
		  h2
		  {
			font-size: 14pt;
		  }
		  h3
		  {
			font-size: 12pt;
		  }
		  table
		  {
			  font-family: Arial, Verdana, Helvetica;
			  border-color: #000000;
			  border-collapse: collapse;
			  font-size: 8pt;
			  margin-left:5; 
			  margin-right:5;
			  text-align: left;
			  border-style: none none none none;
			  border-width:thin;
		  }

		</style>
	  </head>
		<body>
			<xsl:apply-templates select="Report"/>
			
		</body>
</html>

</xsl:template>

<xsl:template match="Report">
	
	<p>
		<span style="margin-left:5; margin-right:5; color:gray;">Entry number: </span>		
		<xsl:value-of select="@ItemId"/>
	</p>
	
	<xsl:apply-templates select="Languages"/>
	
</xsl:template>

<xsl:template match="Languages">
	
	<xsl:for-each select="Language">	 
		<xsl:apply-templates select="."/>					
	</xsl:for-each>	
	
</xsl:template>


<xsl:template match="Language">  

	<table border="0" cellpadding="3" cellspacing="0" width="100%">
	
		<xsl:for-each select=".">			
			<tr>			
				<td width="2%">		
					<img width="28" >
						<xsl:attribute name="src">
							<xsl:value-of select="@FlagFullPath"/>
						</xsl:attribute>
					</img>
				</td>
				
				<td>
					<span>
						<xsl:attribute name="style">													
							<xsl:value-of select="'font-size:10pt;'"/>
							<xsl:value-of select="'font-weight:bold;'"/>
						</xsl:attribute>				
						<span>
							<xsl:choose>
							  <xsl:when test="@IsSource = 'True'">
								<xsl:attribute name="style">
									<xsl:value-of select="'color:Green;'"/>	
								</xsl:attribute>
							  </xsl:when>
							  <xsl:otherwise>
								<xsl:attribute name="style">
									<xsl:value-of select="'color:DarkBlue;'"/>									 
								</xsl:attribute>
							  </xsl:otherwise>
							</xsl:choose>
							
							<xsl:value-of select="@Name"/>
						</span>
					</span>
				</td>
			</tr>
			
			
			<xsl:apply-templates select="Terms">				
				<xsl:with-param name="IsSource" select="@IsSource"/>
			</xsl:apply-templates>
						
		</xsl:for-each>	
	
	</table>
	
</xsl:template>


<xsl:template match="Terms">
	<xsl:param name="IsSource" />
		
	<xsl:apply-templates select="Fields">
		<xsl:with-param name="Structure">Terms</xsl:with-param>
	</xsl:apply-templates>

	<xsl:for-each select="Term">	
		<tr>
			<td>&#160;</td>
			<td>
				<span>
					<xsl:attribute name="style">						
						<xsl:value-of select="'font-size:10pt;'"/>
						<xsl:value-of select="'font-weight:bold;'"/>
					</xsl:attribute>			
					<span>				
						<xsl:choose>
						  <xsl:when test="$IsSource = 'True'">
							<xsl:attribute name="style">
								<xsl:value-of select="'color:Green;'"/>
							</xsl:attribute>
						  </xsl:when>
						  <xsl:otherwise>
							<xsl:attribute name="style">
								<xsl:value-of select="'color:DarkBlue;'"/>		
							</xsl:attribute>
						  </xsl:otherwise>
						</xsl:choose>
						
						<xsl:value-of select="@Value"/>
					</span>
				</span>
			</td>
		</tr> 		

		
		<xsl:apply-templates select="Fields">
			<xsl:with-param name="Structure">Term</xsl:with-param>
		</xsl:apply-templates>
		 	
	</xsl:for-each>

</xsl:template>


<xsl:template match="Fields">
	<xsl:param name="Structure" />

	<xsl:for-each select="Field">	
		<tr>	
			<td>&#160;</td>		
			<xsl:choose>
				<xsl:when test="$Structure='Terms'">
					<td>
						<span style="margin-right:5; color:gray;"><xsl:value-of select="@Name"/>: </span>
						<xsl:value-of select="@Value"/>	
					</td>
				</xsl:when>
				<xsl:when test="$Structure='Term'">
					<td>
						<span style="margin-right:5; color:gray;">&#160;&#160;&#160;&#160;&#160;<xsl:value-of select="@Name"/>: </span>

						<span>
							<xsl:choose>
								<xsl:when test="@Value = 'Deprecated'">
									<xsl:attribute name="style">
										<xsl:value-of select="'color:DarkRed;'"/>
										<xsl:value-of select="'font-weight:bold;'"/>
									</xsl:attribute>
								</xsl:when>
								<xsl:when test="@Value = 'Obsolete'">
									<xsl:attribute name="style">
										<xsl:value-of select="'color:DarkRed;'"/>
										<xsl:value-of select="'font-weight:bold;'"/>
									</xsl:attribute>
								</xsl:when>
								<xsl:when test="@Value = 'Preferred'">
									<xsl:attribute name="style">
										<xsl:value-of select="'color:DarkGreen;'"/>
										<xsl:value-of select="'font-weight:bold;'"/>
									</xsl:attribute>
								</xsl:when>
								<xsl:otherwise>
									<xsl:attribute name="style">
										<xsl:value-of select="'color:Black;'"/>
									</xsl:attribute>
								</xsl:otherwise>
							</xsl:choose>

							<xsl:value-of select="@Value"/>
						</span>
					</td>
				</xsl:when>
				<xsl:otherwise>
					<td> </td>
				</xsl:otherwise>
			</xsl:choose>			
					
		</tr>  
		
	</xsl:for-each>

</xsl:template>

</xsl:stylesheet>
