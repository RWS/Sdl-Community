## MT Enhanced Provider for SDL Trados Studio

## Table of contents 

1. [Intro](#intro)
2. [Features](#features)
3. [XML Structure](#xml-structure)

## Intro

This plugin allows you to retrieve translations from either Microsoft Translator or Google Translate in SDL Trados Studio, with added features.


## Features

* Options to perform a batch find/replace on the source text before sending to the MT provider as well on the translated text returned, using files with a specific XML structure to specify find/replace pairs (current version limited to 20 pairs each for pre-lookup and post-lookup). Use this feature to redact sensitive data from your source text or to improve the results.
* Text-only option: gives you the option of sending and receiving text only, and no tags.
The default setting is to not re-send translated segments: only sends segments with a ‘Not Translated’ segment status; conveniently navigate through your document or review it without increasing your character usage. This is optional and can be changed in the settings form.
* The Microsoft Translator API currently allows users to translate 2M characters/month at no charge after signing up. Users can also create their own custom MT engines on the ‘Microsoft Translator Hub’ and access those engines with this plugin by using the ‘Category ID’ setting.

Once installed, you will now find the option to choose MT Enhanced Plugin for Trados Studio as an MT provider to use in your translation projects.

For more info and screenshots, have a look at the [documentation](https://web.archive.org/web/20160801113006/http://www.linguisticproductions.com/mtenhancedplugin/doc).

## XML Structure

Credits: Initial development done by [Patrick Porter](https://github.com/patrickporter)
