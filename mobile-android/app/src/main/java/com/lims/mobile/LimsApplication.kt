package com.lims.mobile

import android.app.Application
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.preferencesDataStore

val Application.dataStore: DataStore<Preferences> by preferencesDataStore(name = "lims_prefs")

class LimsApplication : Application() {
    companion object {
        lateinit var instance: LimsApplication
            private set
    }

    override fun onCreate() {
        super.onCreate()
        instance = this
    }
}